using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities
{
    public class Session
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string DeviceId { get; set; }
        public required string Country { get; set; }
        public required string Language { get; set; }

        public required string AppVersion { get; set; }
        public required string OsVersion { get; set; }
        public required string DeviceModel { get; set; }

        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;

        public virtual ICollection<CrashReport> Crashes { get; set; } = [];
        public virtual ICollection<MobileEvent> Events { get; set; } = [];
    }
}
