using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelAnalytics.Data.Entities;

public class ProjectMember
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public required string UserEmail { get; set; }
    public string? UserId { get; set; } // Linked from Identity User.Id
    public ProjectRoleType Role { get; set; }
    public bool IsAccepted { get; set; } = false;
    public DateTimeOffset? JoinedAt { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual IdentityUser? User { get; set; } = null!;
}