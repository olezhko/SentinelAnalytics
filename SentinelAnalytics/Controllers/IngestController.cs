using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Models.Dtos;
using SentinelAnalytics.Services;
using System.Text.Json;

namespace SentinelAnalytics.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IngestController(SentinelDbContext db, ICrashNotificationService crashNotificationService) : ControllerBase
{
    [HttpPost("init")]
    public async Task<IActionResult> Init([FromHeader(Name = "X-Sentinel-Key")] string apiKey, InitSessionDto dto)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.ApiKey == apiKey);
        if (project == null) return Unauthorized();

        var session = new Session()
        {
            AppVersion = dto.AppVersion,
            DeviceModel = dto.DeviceModel,
            OsVersion = dto.OsVersion,
            Country = dto.Country,
            Language = dto.Language,
            DeviceId = dto.DeviceId,
            ProjectId = project.Id,
            CreatedAt = DateTime.UtcNow,
        };

        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        return Ok(session.Id);
    }

    [HttpPost("crash")]
    public async Task<IActionResult> PostCrash([FromHeader(Name = "X-Sentinel-Key")] string apiKey, [FromBody] CrashReportDto report)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.ApiKey == apiKey);
        if (project == null) return Unauthorized();

        // Check Plan Limits
        if (await IsLimitReached(project, true))
        {
            return StatusCode(429, "Monthly crash limit reached for this account. Please upgrade your plan.");
        }

        var newReport = new CrashReport()
        {
            ProjectId = project.Id,
            ExceptionName = report.ExceptionName,
            Message = report.Message,
            SessionId = report.SessionId,
            Severity = report.Severity,
            StackTrace = report.StackTrace,
            Timestamp = DateTime.UtcNow,
            UserId = report.UserId,
            PropertiesJson = report.Properties != null ? JsonSerializer.Serialize(report.Properties) : null
        };

        db.CrashReports.Add(newReport);
        await db.SaveChangesAsync();

        await crashNotificationService.NotifyProjectTeamAsync(project.Id, newReport, default);

        return Ok();
    }

    [HttpPost("event")]
    public async Task<IActionResult> PostEvent([FromHeader(Name = "X-Sentinel-Key")] string apiKey, [FromBody] MobileEventDto ev)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.ApiKey == apiKey);
        if (project == null) return Unauthorized();

        // Check Plan Limits
        if (await IsLimitReached(project, false))
        {
            return StatusCode(429, "Monthly event limit reached for this account. Please upgrade your plan.");
        }

        var mobileEvent = new MobileEvent()
        {
            ProjectId = project.Id,
            EventName = ev.EventName,
            SessionId = ev.SessionId,
            Timestamp = DateTime.UtcNow,
            PropertiesJson = ev.Properties != null ? JsonSerializer.Serialize(ev.Properties) : null
        };

        db.MobileEvents.Add(mobileEvent);
        await db.SaveChangesAsync();

        return Ok();
    }

    private async Task<bool> IsLimitReached(Project project, bool isCrash)
    {
        var manager = await db.ProjectMembers
            .Where(pm => pm.ProjectId == project.Id && pm.Role == ProjectRoleType.Manager)
            .FirstOrDefaultAsync();

        if (manager == null) return false;

        var sub = await db.UserDetails
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.UserId == manager.UserId);

        if (sub == null || sub.Plan == null) return false;

        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var managedProjectIds = await db.ProjectMembers
            .Where(pm => pm.UserEmail == manager.UserEmail && pm.Role == ProjectRoleType.Manager)
            .Select(pm => pm.ProjectId)
            .ToListAsync();

        if (isCrash)
        {
            var count = await db.CrashReports.CountAsync(c => managedProjectIds.Contains(c.ProjectId) && c.Timestamp >= startOfMonth);
            return count >= sub.Plan.MaxCrashesPerMonth;
        }
        else
        {
            var count = await db.MobileEvents.CountAsync(e => managedProjectIds.Contains(e.ProjectId) && e.Timestamp >= startOfMonth);
            return count >= sub.Plan.MaxEventsPerMonth;
        }
    }
}