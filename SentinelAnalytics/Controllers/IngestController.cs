using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Models.Dtos;
using System.Text.Json;

namespace SentinelAnalytics.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IngestController(SentinelDbContext db) : ControllerBase
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
            ProjectId = project.Id
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

        return Ok();
    }

    [HttpPost("event")]
    public async Task<IActionResult> PostEvent([FromHeader(Name = "X-Sentinel-Key")] string apiKey, [FromBody] MobileEventDto ev)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.ApiKey == apiKey);
        if (project == null) return Unauthorized();

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
}