using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities
{
    public class UserDetail : AuditableEntity
    {
        public required string UserId { get; set; }
        public Guid PlanId { get; set; }
        public DateTime StartDate { get; set; }


        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; } = null!;

        [ForeignKey("PlanId")]
        public virtual PricingPlan Plan { get; set; } = null!;
    }
}
