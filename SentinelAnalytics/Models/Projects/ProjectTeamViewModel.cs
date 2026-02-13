using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Models.Projects;

public class ProjectTeamViewModel
{
    public List<ProjectMember> Memberships { get; set; } = new List<ProjectMember>();
    public List<ProjectMember> PendingInvites { get; set; } = new List<ProjectMember>();
}