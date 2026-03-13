using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Models.Analytics;

namespace SentinelAnalytics.Controllers
{
    [Authorize]
    public class AnalyticsController(SentinelDbContext db) : Controller
    {
        public async Task<IActionResult> Index(Guid projectId, string? appVersion, DateTime? dateFrom, DateTime? dateTo)
        {
            var project = await db.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            var from = dateFrom ?? DateTime.UtcNow.AddDays(-30);
            var to = dateTo ?? DateTime.UtcNow;

            var viewModel = new AnalyticsViewModel
            {
                ProjectId = projectId,
                ProjectName = project.Name,
                SelectedVersion = appVersion ?? "All",
                DateFrom = from,
                DateTo = to,
                AvailableVersions = await db.MobileEvents
                    .Include(e => e.Session)
                    .AsNoTracking()
                    .Where(e => e.ProjectId == projectId)
                    .Select(e => e.Session.AppVersion)
                    .Distinct()
                    .OrderByDescending(v => v)
                    .ToListAsync()
            };

            var eventsQuery = db.MobileEvents
                .AsNoTracking()
                .Where(e => e.ProjectId == projectId && e.Timestamp >= from && e.Timestamp <= to);

            // Apply version filter if specified
            if (!string.IsNullOrEmpty(appVersion) && appVersion != "All")
            {
                eventsQuery = eventsQuery.Where(e => e.Session.AppVersion == appVersion);
            }

            // 1. Active Users (Unique Sessions per day)
            var dailyActivity = await eventsQuery
                .GroupBy(e => e.Timestamp.Date)
                .Select(g => new {
                    Date = g.Key,
                    UserCount = g.Select(x => x.SessionId).Distinct().Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            viewModel.ActiveUsersPerDay = dailyActivity.Select(d => new ChartDataPoint
            {
                Label = d.Date.ToString("MMM dd"),
                Value = d.UserCount
            }).ToList();

            // 2. Session Duration Distribution
            // Duration is calculated as max(timestamp) - min(timestamp) for each session
            var sessionTimes = await eventsQuery
                .GroupBy(e => e.SessionId)
                .Select(g => new {
                    Start = g.Min(x => x.Timestamp),
                    End = g.Max(x => x.Timestamp)
                })
                .ToListAsync();

            var durationsInMinutes = sessionTimes.Select(s => (s.End - s.Start).TotalMinutes).ToList();

            viewModel.SessionDurations = new List<ChartDataPoint> {
                new ChartDataPoint { Label = "< 1 min", Value = durationsInMinutes.Count(d => d < 1) },
                new ChartDataPoint { Label = "< 5 min", Value = durationsInMinutes.Count(d => d >= 1 && d < 5) },
                new ChartDataPoint { Label = "< 30 min", Value = durationsInMinutes.Count(d => d >= 5 && d < 30) },
                new ChartDataPoint { Label = "30m - 1h", Value = durationsInMinutes.Count(d => d >= 30 && d < 60) },
                new ChartDataPoint { Label = "> 1h", Value = durationsInMinutes.Count(d => d >= 60) }
            };

            // 3. Country & 4. Language Stats
            var countries = await db.Sessions
                .AsNoTracking()
                .Where(s => s.ProjectId == projectId && s.CreatedAt > from && s.CreatedAt < to)
                .GroupBy(s => s.Country ?? "Unknown")
                .Select(g => new { Key = g.Key, Value = g.Count() })
                .ToListAsync();

            var languages = await db.Sessions
                .AsNoTracking()
                .Where(s => s.ProjectId == projectId && s.CreatedAt > from && s.CreatedAt < to)
                .GroupBy(s => s.Language ?? "Unknown")
                .Select(g => new { Key = g.Key, Value = g.Count() })
                .ToListAsync();

            viewModel.RegionalStats = countries
                .OrderByDescending(x => x.Value)
                .Select(x => new ChartDataPoint { Label = x.Key, Value = x.Value })
                .ToList();

            viewModel.LanguageStats = languages
                .OrderByDescending(x => x.Value)
                .Select(x => new ChartDataPoint { Label = x.Key, Value = x.Value })
                .ToList();

            return View(viewModel);
        }
    }
}
