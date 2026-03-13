using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Models.Projects;

namespace SentinelAnalytics.Controllers
{
    [Authorize]
    public class ProjectController(
        UserManager<IdentityUser> userManager,
        SentinelDbContext _db
        ) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);

            var projectsInfo = await _db.ProjectMembers
               .Include(pm => pm.Project)
               .Where(pm => pm.UserId == userId)
               .Select(item => new ProjectMembershipViewModel
               {
                   Project = item.Project,
                   IsAccepted = item.IsAccepted,
                   IsManager = item.Project.UserId == userId,
                   MemberId = item.Id
               })
               .ToListAsync();

            return View("Index", projectsInfo);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptInvite(Guid inviteId)
        {
            var invite = await _db.ProjectMembers.FindAsync(inviteId);
            if (invite == null)
                return NotFound();

            var userId = userManager.GetUserId(User);
            invite.IsAccepted = true;
            invite.JoinedAt = DateTime.UtcNow;
            invite.UserId = userId;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var user = await userManager.GetUserAsync(User);

            var project = await _db.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            var isManager = project.Members
                .Any(m => m.UserEmail == user.Email && m.Role == ProjectRoleType.Manager);

            if (!isManager)
                return Forbid();

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(string name, string platform)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Project name is required.");

            var user = await userManager.GetUserAsync(User)
                ?? throw new InvalidDataException("User not login");

            var newProject = new Project { UserId = user.Id, Name = name, Platform = platform, CreatedDate = DateTime.UtcNow };
            _db.Projects.Add(newProject);

            var member = new ProjectMember
            {
                ProjectId = newProject.Id,
                UserEmail = user.Email,
                Role = ProjectRoleType.Manager,
                IsAccepted = true,
                UserId = user.Id,
                JoinedAt = DateTime.UtcNow
            };
            _db.ProjectMembers.Add(member);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
