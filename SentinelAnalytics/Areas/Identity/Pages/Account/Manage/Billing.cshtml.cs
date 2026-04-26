// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SentinelAnalytics.Areas.Identity.Pages.Account.Manage
{
    public class BillingModel(
        SentinelDbContext dbContext,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager) : PageModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            public Guid UserSubscriptionId { get; set; }
            public bool NotifyOnCritical { get; set; }
            public bool NotifyOnError { get; set; }
            public bool NotifyOnRegression { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await userManager.GetUserNameAsync(user);
            Username = userName;

            var data = await dbContext.UserSubscriptions
                .FirstOrDefaultAsync(item => item.UserId == user.Id) 
                    ?? new UserSubscription
                        {
                            UserId = user.Id,
                            NotifyOnCritical = true,
                            NotifyOnError = true,
                            NotifyOnRegression = true,
                        };

            Input = new InputModel
            {
                NotifyOnCritical = data.NotifyOnCritical,
                NotifyOnError = data.NotifyOnError,
                NotifyOnRegression = data.NotifyOnRegression,
                UserSubscriptionId = data.Id
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.UserSubscriptionId == Guid.Empty)
            {
                dbContext.UserSubscriptions.Add(new UserSubscription()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    NotifyOnCritical = Input.NotifyOnCritical,
                    NotifyOnError = Input.NotifyOnError,
                    NotifyOnRegression = Input.NotifyOnRegression,
                });
            }
            else
            {
                var subs = await dbContext.UserSubscriptions.FindAsync(Input.UserSubscriptionId);
                subs.NotifyOnRegression = Input.NotifyOnRegression;
                subs.NotifyOnCritical = Input.NotifyOnCritical;
                subs.NotifyOnError = Input.NotifyOnError;
            }

            await dbContext.SaveChangesAsync();
            await signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
