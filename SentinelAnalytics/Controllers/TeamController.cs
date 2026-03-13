using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;

namespace SentinelAnalytics.Controllers;

[Authorize]
public class TeamController(UserManager<IdentityUser> userManager, SentinelDbContext db) : Controller
{
    public async Task<IActionResult> Index(Guid projectId)
    {
        var userId = userManager.GetUserId(User);

        var project = await db.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return NotFound();

        // Check if user belongs to project
        var membership = project.Members.FirstOrDefault(m => m.UserId == userId && m.IsAccepted);
        if (membership == null) 
            return Forbid();

        ViewBag.IsManager = membership.Role == ProjectRoleType.Manager;
        return View(project);
    }

    [HttpPost]
    public async Task<IActionResult> InviteDeveloper(Guid projectId, string email)
    {
        var userId = userManager.GetUserId(User);

        var isManager = await db.ProjectMembers
            .AnyAsync(m => m.ProjectId == projectId 
            && m.UserId == userId
            && m.Role == ProjectRoleType.Manager);

        if (!isManager) 
            return Forbid();

        var existing = await db.ProjectMembers.AnyAsync(m => m.ProjectId == projectId && m.UserEmail == email);
        if (existing) 
            return BadRequest("User is already a member or invited.");

        var invite = new ProjectMember
        {
            ProjectId = projectId,
            UserEmail = email,
            Role = ProjectRoleType.Developer,
            IsAccepted = false
        };

        db.ProjectMembers.Add(invite);
        await db.SaveChangesAsync();

        // ToDo: Send email with invite

        return RedirectToAction(nameof(Index), new { projectId });
    }

    [HttpPost]
    public async Task<IActionResult> RevokeAccess(Guid memberId)
    {
        var userId = userManager.GetUserId(User);

        var member = await db.ProjectMembers.FindAsync(memberId);
        if (member == null) return NotFound();

        var isManager = await db.ProjectMembers
            .AnyAsync(m => m.ProjectId == member.ProjectId && m.UserId == userId && m.Role == ProjectRoleType.Manager);

        if (!isManager || member.Role == ProjectRoleType.Manager) return Forbid();

        db.ProjectMembers.Remove(member);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { projectId = member.ProjectId });
    }
}