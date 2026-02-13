namespace SentinelAnalytics.Models.Projects;

public class ProjectMembershipViewModel
{
    public required SentinelAnalytics.Data.Entities.Project Project { get; set; }
    public bool IsAccepted { get; set; }
    public bool IsManager { get; set; }
    public Guid MemberId { get; set; }
}