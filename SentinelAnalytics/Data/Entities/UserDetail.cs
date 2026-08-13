using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities
{
    public class UserDetail : AuditableEntity
    {
        public required string UserId { get; set; }
        public Guid PlanId { get; set; }
        public DateTime StartDate { get; set; }

        // Payment method (last 4 only — full card number never stored)
        public string? CardholderName { get; set; }
        public string? CardLastFour { get; set; }
        public string? CardExpiry { get; set; }
        public string? CardBrand { get; set; }

        // Billing address
        public string? BillingAddress { get; set; }
        public string? BillingCity { get; set; }
        public string? BillingState { get; set; }
        public string? BillingZip { get; set; }
        public string? BillingCountry { get; set; }

        // Stripe
        public string? StripeCustomerId { get; set; }
        public string? StripeSubscriptionId { get; set; }
        public string? StripeSubscriptionStatus { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; } = null!;

        [ForeignKey("PlanId")]
        public virtual PricingPlan Plan { get; set; } = null!;
    }
}
