#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Services;
using System.ComponentModel.DataAnnotations;

namespace SentinelAnalytics.Areas.Identity.Pages.Account.Manage
{
    public class BillingModel(
        SentinelDbContext dbContext,
        UserManager<IdentityUser> userManager,
        IStripeService stripeService,
        IConfiguration configuration) : PageModel
    {
        public string Username { get; set; }
        public string StripePublishableKey { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool HasActivePlan { get; set; }
        public UserDetail CurrentUserDetail { get; set; }
        public List<PricingPlan> AvailablePlans { get; set; }

        public class InputModel
        {
            public Guid SelectedPlanId { get; set; }

            // Set by Stripe.js — raw card never reaches the server
            public string PaymentMethodId { get; set; }

            [Display(Name = "Billing Address")]
            public string BillingAddress { get; set; }

            [Display(Name = "City")]
            public string BillingCity { get; set; }

            [Display(Name = "State / Region")]
            public string BillingState { get; set; }

            [Display(Name = "ZIP / Postal Code")]
            public string BillingZip { get; set; }

            [Display(Name = "Country")]
            public string BillingCountry { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            Username = await userManager.GetUserNameAsync(user);
            StripePublishableKey = configuration["Stripe:PublishableKey"];
            AvailablePlans = await dbContext.PricingPlans.OrderBy(p => p.Price).ToListAsync();

            CurrentUserDetail = await dbContext.UserDetails
                .Include(d => d.Plan)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            HasActivePlan = CurrentUserDetail != null;

            if (HasActivePlan)
            {
                Input = new InputModel
                {
                    SelectedPlanId = CurrentUserDetail.PlanId,
                    BillingAddress = CurrentUserDetail.BillingAddress,
                    BillingCity = CurrentUserDetail.BillingCity,
                    BillingState = CurrentUserDetail.BillingState,
                    BillingZip = CurrentUserDetail.BillingZip,
                    BillingCountry = CurrentUserDetail.BillingCountry,
                };
            }
            else
            {
                Input = new InputModel();
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            if (Input.SelectedPlanId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Please select a plan.");
                await LoadAsync(user);
                return Page();
            }

            var plan = await dbContext.PricingPlans.FindAsync(Input.SelectedPlanId);
            if (plan == null)
            {
                ModelState.AddModelError(string.Empty, "Selected plan not found.");
                await LoadAsync(user);
                return Page();
            }

            var isPaidPlan = plan.Price > 0;

            // For paid plans, require either a new payment method or an existing one on file
            var existing = await dbContext.UserDetails.FirstOrDefaultAsync(d => d.UserId == user.Id);
            var hasExistingCard = existing?.CardLastFour != null;

            if (isPaidPlan && string.IsNullOrWhiteSpace(Input.PaymentMethodId) && !hasExistingCard)
            {
                ModelState.AddModelError(string.Empty, "A payment method is required for paid plans.");
                await LoadAsync(user);
                return Page();
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            try
            {
                await ProcessSubscriptionAsync(user, plan, existing);
            }
            catch (Stripe.StripeException ex)
            {
                ModelState.AddModelError(string.Empty, ex.StripeError?.Message ?? ex.Message);
                await LoadAsync(user);
                return Page();
            }

            StatusMessage = "Billing information saved.";
            return RedirectToPage();
        }

        private async Task ProcessSubscriptionAsync(IdentityUser user, PricingPlan plan, UserDetail existing)
        {
            var isPaidPlan = plan.Price > 0;
            var hasNewPaymentMethod = !string.IsNullOrWhiteSpace(Input.PaymentMethodId);

            // Get Stripe payment method details if a new card was provided
            StripePaymentMethodDetails pmDetails = null;
            if (hasNewPaymentMethod)
            {
                pmDetails = await stripeService.GetPaymentMethodDetailsAsync(Input.PaymentMethodId);
            }

            if (existing == null)
            {
                // New subscriber
                string stripeCustomerId = null;
                string stripeSubscriptionId = null;
                string stripeStatus = null;

                if (isPaidPlan && hasNewPaymentMethod)
                {
                    stripeCustomerId = await stripeService.GetOrCreateCustomerAsync(user.Id, user.Email);
                    var result = await stripeService.SubscribeAsync(stripeCustomerId, plan.StripePriceId, Input.PaymentMethodId);
                    stripeSubscriptionId = result.SubscriptionId;
                    stripeStatus = result.Status;
                }

                dbContext.UserDetails.Add(new UserDetail
                {
                    UserId = user.Id,
                    PlanId = plan.Id,
                    StartDate = DateTime.UtcNow,
                    CardholderName = pmDetails?.CardholderName,
                    CardLastFour = pmDetails?.LastFour,
                    CardExpiry = pmDetails?.Expiry,
                    CardBrand = pmDetails?.Brand,
                    BillingAddress = Input.BillingAddress,
                    BillingCity = Input.BillingCity,
                    BillingState = Input.BillingState,
                    BillingZip = Input.BillingZip,
                    BillingCountry = Input.BillingCountry,
                    StripeCustomerId = stripeCustomerId,
                    StripeSubscriptionId = stripeSubscriptionId,
                    StripeSubscriptionStatus = stripeStatus ?? (isPaidPlan ? null : "active"),
                });
            }
            else
            {
                // Existing subscriber — update
                var planChanged = existing.PlanId != plan.Id;

                if (isPaidPlan)
                {
                    // Ensure Stripe customer exists
                    existing.StripeCustomerId ??= await stripeService.GetOrCreateCustomerAsync(user.Id, user.Email);

                    if (planChanged && !string.IsNullOrEmpty(existing.StripeSubscriptionId))
                    {
                        // Change plan on existing subscription
                        var result = await stripeService.UpdateSubscriptionPlanAsync(existing.StripeSubscriptionId, plan.StripePriceId);
                        existing.StripeSubscriptionId = result.SubscriptionId;
                        existing.StripeSubscriptionStatus = result.Status;
                    }
                    else if (string.IsNullOrEmpty(existing.StripeSubscriptionId))
                    {
                        // Had free plan, now upgrading — need a payment method
                        if (!hasNewPaymentMethod)
                            throw new InvalidOperationException("Payment method required when upgrading from free plan.");

                        var result = await stripeService.SubscribeAsync(existing.StripeCustomerId, plan.StripePriceId, Input.PaymentMethodId);
                        existing.StripeSubscriptionId = result.SubscriptionId;
                        existing.StripeSubscriptionStatus = result.Status;
                    }

                    if (hasNewPaymentMethod && !string.IsNullOrEmpty(existing.StripeSubscriptionId))
                    {
                        await stripeService.UpdatePaymentMethodAsync(existing.StripeCustomerId, existing.StripeSubscriptionId, Input.PaymentMethodId);
                    }
                }
                else if (planChanged && !string.IsNullOrEmpty(existing.StripeSubscriptionId))
                {
                    // Downgrading to free — cancel Stripe subscription
                    await stripeService.CancelSubscriptionAsync(existing.StripeSubscriptionId);
                    existing.StripeSubscriptionId = null;
                    existing.StripeSubscriptionStatus = "canceled";
                }

                existing.PlanId = plan.Id;
                if (planChanged) existing.StartDate = DateTime.UtcNow;

                if (pmDetails != null)
                {
                    existing.CardholderName = pmDetails.CardholderName ?? existing.CardholderName;
                    existing.CardLastFour = pmDetails.LastFour;
                    existing.CardExpiry = pmDetails.Expiry;
                    existing.CardBrand = pmDetails.Brand;
                }

                if (!string.IsNullOrWhiteSpace(Input.BillingAddress)) existing.BillingAddress = Input.BillingAddress;
                if (!string.IsNullOrWhiteSpace(Input.BillingCity)) existing.BillingCity = Input.BillingCity;
                if (Input.BillingState != null) existing.BillingState = Input.BillingState;
                if (!string.IsNullOrWhiteSpace(Input.BillingZip)) existing.BillingZip = Input.BillingZip;
                if (!string.IsNullOrWhiteSpace(Input.BillingCountry)) existing.BillingCountry = Input.BillingCountry;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
