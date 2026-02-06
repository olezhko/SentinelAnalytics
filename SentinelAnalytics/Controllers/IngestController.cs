using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Models.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SentinelAnalytics.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IngestController(SentinelDbContext db) : ControllerBase
{
    [HttpPost("crash")]
    public async Task<IActionResult> PostCrash([FromHeader(Name = "X-Sentinel-Key")] string apiKey, [FromBody] CrashReportDto report)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.ApiKey == apiKey);
        if (project == null) return Unauthorized();

        var newReport = new CrashReport()
        {
            ProjectId = project.Id,
            AppVersion = report.AppVersion,
            DeviceModel = report.DeviceModel,
            ExceptionName = report.ExceptionName,
            Message = report.Message,
            OsVersion = report.OsVersion,
            SessionId = report.SessionId,
            Severity = report.Severity,
            StackTrace = report.StackTrace,
            Timestamp = DateTime.UtcNow,
            UserId = report.UserId ?? string.Empty
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
            PropertiesJson = JsonSerializer.Serialize(ev.Properties)
        };

        db.MobileEvents.Add(mobileEvent);
        await db.SaveChangesAsync();
        return Ok();
    }
}