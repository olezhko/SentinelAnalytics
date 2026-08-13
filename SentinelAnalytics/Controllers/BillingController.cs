using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Models.Dashboard;

namespace SentinelAnalytics.Controllers
{
    public class BillingController(SentinelDbContext db, UserManager<IdentityUser> userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            var sub = await db.UserDetails.Include(s => s.Plan).FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (sub == null)
            {
                var freePlan = await db.PricingPlans.FirstOrDefaultAsync(p => p.Price == 0);
                sub = new UserDetail { UserId = user.Id, PlanId = freePlan.Id, Plan = freePlan, StartDate = DateTime.UtcNow };
                db.UserDetails.Add(sub);
                await db.SaveChangesAsync();
            }

            // Calculate usage
            var managedProjectIds = await db.ProjectMembers
                .Where(pm => pm.UserId == user.Id && pm.Role == ProjectRoleType.Manager)
                .Select(pm => pm.ProjectId)
                .ToListAsync();

            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            var viewModel = new BillingViewModel
            {
                CurrentPlan = sub.Plan,
                AvailablePlans = await db.PricingPlans.OrderBy(p => p.Price).ToListAsync(),
                CurrentProjectCount = managedProjectIds.Count,
                CurrentEventCountMonth = await db.MobileEvents
                    .CountAsync(e => managedProjectIds.Contains(e.ProjectId) && e.Timestamp >= startOfMonth),
                CurrentCrashCountMonth = await db.CrashReports
                    .CountAsync(c => managedProjectIds.Contains(c.ProjectId) && c.Timestamp >= startOfMonth),
                StartDate = sub.StartDate,
                StripeSubscriptionStatus = sub.StripeSubscriptionStatus,
                CardLastFour = sub.CardLastFour,
                CardExpiry = sub.CardExpiry,
                CardBrand = sub.CardBrand,
                CardholderName = sub.CardholderName,
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ChangePlan()
        {
            // Plan changes with payment go through the Billing identity page
            return RedirectToPage("/Account/Manage/Billing", new { area = "Identity" });
        }
    }
}
