using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Models;
using SentinelAnalytics.Services;

namespace SentinelAnalytics.Controllers;

[Authorize]
public class DashboardController(SentinelDbContext db, IGeminiService ai) : Controller
{
    public IActionResult Demo()
    {
        var now = DateTime.UtcNow;
        // Generates rich mock data for demonstration purposes
        var demoStats = new DashboardStatsViewModel
        {
            TotalCrashes = 142,
            ProjectName = "Demo",
            ActiveUsersCount = 28450,
            DailyTrends = new List<DailyStat>
            {
                new DailyStat { Date = now.AddDays(-6).ToString("MMM dd"), Count = 12 },
                new DailyStat { Date = now.AddDays(-5).ToString("MMM dd"), Count = 18 },
                new DailyStat { Date = now.AddDays(-4).ToString("MMM dd"), Count = 8 },
                new DailyStat { Date = now.AddDays(-3).ToString("MMM dd"), Count = 25 },
                new DailyStat { Date = now.AddDays(-2).ToString("MMM dd"), Count = 14 },
                new DailyStat { Date = now.AddDays(-1).ToString("MMM dd"), Count = 32 },
                new DailyStat { Date = now.ToString("MMM dd"), Count = 19 }
            },
            RecentCrashes = new List<CrashReport>
            {
                new CrashReport {
                    SessionId = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid(),
                    ExceptionName = "NullReferenceException",
                    Message = "Object reference not set to an instance of an object at PaymentGateway.AuthorizeTransaction",
                    AppVersion = "3.2.0",
                    OsVersion = "iOS 17.5.1",
                    DeviceModel = "iPhone 15 Pro",
                    Timestamp = DateTime.UtcNow.AddMinutes(-12),
                    Severity = Severity.Critical,
                    StackTrace = "at Sentinel.Mobile.PaymentGateway.AuthorizeTransaction(Amount val) in Gateway.cs:line 442\nat Sentinel.Mobile.Checkout.Confirm() in CheckoutViewModel.cs:line 89"
                },
                new CrashReport {
                    SessionId = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid(),
                    ExceptionName = "SQLiteException",
                    Message = "Database is locked. Unable to perform write operation during sync.",
                    AppVersion = "3.1.8",
                    OsVersion = "Android 14",
                    DeviceModel = "Google Pixel 8 Pro",
                    Timestamp = DateTime.UtcNow.AddHours(-2),
                    Severity = Severity.Error,
                    StackTrace = "at Microsoft.Data.Sqlite.SqliteException.ThrowExceptionForRC(Int32 rc, sqlite3 db)\nat Microsoft.Data.Sqlite.SqliteCommand.ExecuteNonQuery()"
                },
                new CrashReport {
                    SessionId = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid(),
                    ExceptionName = "IndexOutOfRangeException",
                    Message = "Index was outside the bounds of the array at FeedAdapter.OnBindViewHolder",
                    AppVersion = "3.2.0",
                    OsVersion = "Android 13",
                    DeviceModel = "Samsung Galaxy S23",
                    Timestamp = DateTime.UtcNow.AddHours(-5),
                    Severity = Severity.Warning,
                    StackTrace = "at Sentinel.Mobile.Adapters.FeedAdapter.OnBindViewHolder(ViewHolder holder, Int32 position)\nat Android.Widget.RecyclerView.Bind()"
                },
                new CrashReport {
                    SessionId = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid(),
                    ExceptionName = "TaskCanceledException",
                    Message = "A task was canceled while waiting for the ImageBuffer to flush.",
                    AppVersion = "3.2.1-beta",
                    OsVersion = "iOS 18.0",
                    DeviceModel = "iPad Pro M4",
                    Timestamp = DateTime.UtcNow.AddHours(-8),
                    Severity = Severity.Info,
                    StackTrace = "at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\nat Sentinel.Mobile.Media.ImageLoader.LoadAsync(String url)"
                }
            }
        };

        return View(demoStats);
    }

    public async Task<IActionResult> Index(Guid projectId)
    {
        var project = await db.Projects.FirstOrDefaultAsync(i => i.Id == projectId);
        var query = db.CrashReports.AsQueryable()
            .Where(c => c.ProjectId == projectId);

        var stats = new DashboardStatsViewModel
        {
            ProjectName = project!.Name,
            TotalCrashes = await query.CountAsync(),
            DailyTrends = await query
                .GroupBy(c => c.Timestamp.Date)
                .OrderBy(g => g.Key)
                .Select(g => new DailyStat { Date = g.Key.ToString("MMM dd"), Count = g.Count() })
                .ToListAsync(),
            RecentCrashes = await query
                .OrderByDescending(c => c.Timestamp)
                .Take(10)
                .ToListAsync(),
            ActiveUsersCount = 1284 // Mocked for UI
        };

        return View(stats);
    }

    public async Task<IActionResult> CrashDetails(Guid id)
    {
        var crash = await db.CrashReports
            .Include(c => c.Project)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (crash == null) return NotFound();

        // KEY FEATURE: Correlate all events during the session
        var sessionBreadcrumbs = await db.MobileEvents
            .Where(e => e.SessionId == crash.SessionId && e.Timestamp <= crash.Timestamp)
            .OrderBy(e => e.Timestamp)
            .ToListAsync();

        ViewBag.Breadcrumbs = sessionBreadcrumbs;
        ViewBag.AIAnalysis = await ai.AnalyzeCrashAsync(crash);

        return View(crash);
    }
}