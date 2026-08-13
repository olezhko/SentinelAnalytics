using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Models.Dashboard;
using SentinelAnalytics.Services;
using System.Text;

namespace SentinelAnalytics.Controllers;

[Authorize]
public class DashboardController(
    SentinelDbContext db, 
    UserManager<IdentityUser> userManager,
    IGeminiService ai) : Controller
{
    [AllowAnonymous]
    public IActionResult Demo()
    {
        var now = DateTime.UtcNow;
        // Generates rich mock data for demonstration purposes
        var demoStats = new DashboardStatsViewModel
        {
            TotalCrashes = 142,
            ProjectName = "Demo",
            ActiveSessionsCount = 28450,
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
                    SessionId = Guid.NewGuid(),
                    Session = new Session {
                        DeviceId = "abc123",
                        Country = "USA",
                        Language = "en",
                        AppVersion = "3.2.0",
                        OsVersion = "iOS 17.5.1",
                        DeviceModel = "iPhone 15 Pro"
                    },
                    Id = Guid.NewGuid(),
                    ExceptionName = "NullReferenceException",
                    Message = "Object reference not set to an instance of an object at PaymentGateway.AuthorizeTransaction",
                    Timestamp = DateTime.UtcNow.AddMinutes(-12),
                    Severity = Severity.Critical,
                    StackTrace = "at Sentinel.Mobile.PaymentGateway.AuthorizeTransaction(Amount val) in Gateway.cs:line 442\nat Sentinel.Mobile.Checkout.Confirm() in CheckoutViewModel.cs:line 89"
                },
                new CrashReport {
                    SessionId = Guid.NewGuid(),
                    Session = new Session {
                        DeviceId = "def456",
                        Country = "Germany",
                        Language = "de",
                        AppVersion = "3.1.8",
                        OsVersion = "Android 14",
                        DeviceModel = "Google Pixel 8 Pro"
                    },
                    Id = Guid.NewGuid(),
                    ExceptionName = "SQLiteException",
                    Message = "Database is locked. Unable to perform write operation during sync.",
                    Timestamp = DateTime.UtcNow.AddHours(-2),
                    Severity = Severity.Error,
                    StackTrace = "at Microsoft.Data.Sqlite.SqliteException.ThrowExceptionForRC(Int32 rc, sqlite3 db)\nat Microsoft.Data.Sqlite.SqliteCommand.ExecuteNonQuery()"
                },
                new CrashReport {
                    SessionId = Guid.NewGuid(),
                    Session = new Session {
                        DeviceId = "ghi789",
                        Country = "Japan",
                        Language = "ja",
                        AppVersion = "3.2.1-beta",
                        OsVersion = "Android 13",
                        DeviceModel = "Sony Xperia 1 IV"
                    },
                    Id = Guid.NewGuid(),
                    ExceptionName = "IndexOutOfRangeException",
                    Message = "Index was outside the bounds of the array at FeedAdapter.OnBindViewHolder",
                    Timestamp = DateTime.UtcNow.AddHours(-5),
                    Severity = Severity.Warning,
                    StackTrace = "at Sentinel.Mobile.Adapters.FeedAdapter.OnBindViewHolder(ViewHolder holder, Int32 position)\nat Android.Widget.RecyclerView.Bind()"
                },
                new CrashReport {
                    SessionId = Guid.NewGuid(),
                    Session = new Session {
                        DeviceId = "jkl012",
                        Country = "UK",
                        Language = "en",
                        AppVersion = "3.2.1-beta",
                        OsVersion = "iOS 18.0",
                        DeviceModel = "iPad Pro M4"
                    },
                    Id = Guid.NewGuid(),
                    ExceptionName = "TaskCanceledException",
                    Message = "A task was canceled while waiting for the ImageBuffer to flush.",
                    Timestamp = DateTime.UtcNow.AddHours(-8),
                    Severity = Severity.Warning,
                    StackTrace = "at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\nat Sentinel.Mobile.Media.ImageLoader.LoadAsync(String url)"
                }
            },
            AvailableVersions = new List<string> { "3.2.1-beta", "3.2.0", "3.1.8" }
        };

        return View(demoStats);
    }

    public async Task<IActionResult> Index(Guid projectId, string? appVersion, string? timePeriod, Severity? severity, string? resolutionStatus,
        string? searchQuery)
    {
        var project = await db.Projects.FirstOrDefaultAsync(i => i.Id == projectId);

        if (!await IsUserInProjectTeam(project!))
        {
            return Forbid();
        }

        var query = GetFilteredCrashesQuery(projectId, appVersion, timePeriod, severity, resolutionStatus, searchQuery);

        // Get available versions for the dropdown
        var allVersions = await query
            .Select(c => c.Session.AppVersion)
            .Distinct()
            .OrderByDescending(v => v)
            .ToListAsync();

        var totalSessions = await db.MobileEvents
            .Where(item => item.ProjectId == projectId)
            .Select(c => c.SessionId)
            .Distinct()
            .CountAsync();

        var totalCrashes = await query.CountAsync();
        var impactedUsers = await query.Select(c => c.UserId ?? c.Session.DeviceId).Distinct().CountAsync();
        var estimatedActiveUsers = totalSessions;
        var crashFreeRate = 100.0 - ((double)impactedUsers / estimatedActiveUsers * 100.0);
        var recentCrashesRaw = await query
            .OrderByDescending(c => c.Timestamp)
            .Take(50)
            .ToListAsync();

        // Group by Exception to find regressions/counts
        var groupedCrashes = recentCrashesRaw
            .GroupBy(c => new ExceptionStackTrace( c.ExceptionName, c.StackTrace))
            .Select(g => new CrashReportSummary
            {
                Exception = g.Key,
                Report = g.OrderByDescending(x => x.Timestamp).First(),
                OccurrenceCount = g.Count(),
                AffectedUsersCount = g.Select(x => x.UserId ?? x.Session.DeviceId).Distinct().Count(),
                IsRegression = g.Any(x => x.IsResolved && x.Timestamp > (x.ResolvedAt ?? DateTime.MinValue))
            }).ToList();

        var stats = new DashboardStatsViewModel
        {
            TotalCrashes = totalCrashes,
            UniqueSessionsImpacted = impactedUsers,
            CrashFreeUserRate = Math.Max(0, Math.Min(100, crashFreeRate)),
            ProjectName = project!.Name,
            DailyTrends = await query
                .GroupBy(c => c.Timestamp.Date)
                .OrderBy(g => g.Key)
                .Select(g => new DailyStat { Date = g.Key.ToString("MMM dd"), Count = g.Count() })
                .ToListAsync(),
            RecentCrashes = await query
                .OrderByDescending(c => c.Timestamp)
                .Take(25)
                .ToListAsync(),
            ActiveSessionsCount = totalSessions,
            AvailableVersions = allVersions,
            SelectedVersion = appVersion,
            SelectedSeverity = severity,
            SelectedPeriod = timePeriod,
            SelectedResolution = resolutionStatus,
            CurrentProjectId = projectId,
            SearchQuery = searchQuery
        };

        return View(stats);
    }

    private async Task<bool> IsUserInProjectTeam(Project project)
    {
        var userId = userManager.GetUserId(User);

        return await db.ProjectMembers.AnyAsync(item => item.ProjectId == project.Id
            && item.UserId == userId);
    }

    public async Task<IActionResult> CrashDetails(Guid id)
    {
        var crash = await db.CrashReports
            .Include(c => c.Project)
            .Include(c => c.Session)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (crash == null) return NotFound();

        var sessionBreadcrumbs = await db.MobileEvents.AsNoTracking()
            .Include(c => c.Session)
            .Where(e => e.SessionId == crash.SessionId && e.Timestamp <= crash.Timestamp)
            .OrderBy(e => e.Timestamp)
            .ToListAsync();

        ViewBag.Breadcrumbs = sessionBreadcrumbs;
        ViewBag.AIAnalysis = await ai.AnalyzeCrashAsync(crash);

        return View(crash);
    }

    [HttpPost]
    public async Task<IActionResult> Resolve(Guid id, string comment)
    {
        var crash = await db.CrashReports.FindAsync(id);
        if (crash == null) return NotFound();

        crash.IsResolved = true;
        crash.ResolvedAt = DateTime.UtcNow;
        crash.ResolutionComment = comment;

        await db.SaveChangesAsync();

        return RedirectToAction(nameof(CrashDetails), new { id = id });
    }

    public async Task<IActionResult> ExportReport(Guid projectId, string? appVersion, string? timePeriod, Severity? severity, string? resolutionStatus,
        string? searchQuery)
    {
        var query = GetFilteredCrashesQuery(projectId, appVersion, timePeriod, severity, resolutionStatus, searchQuery);
        var crashes = await query.OrderByDescending(c => c.Timestamp).ToListAsync();

        var builder = new StringBuilder();
        builder.AppendLine("ID,Timestamp,Severity,Exception,Message,AppVersion,OS,Device,User,Resolved,ResolutionDate");

        foreach (var crash in crashes)
        {
            string safeMsg = crash.Message?.Replace(",", ";").Replace("\n", " ") ?? "";
            builder.AppendLine($"{crash.Id},{crash.Timestamp:yyyy-MM-dd HH:mm:ss},{crash.Severity},{crash.ExceptionName},{safeMsg},{crash.Session.AppVersion},{crash.Session.OsVersion},{crash.Session.DeviceModel},{crash.UserId},{crash.IsResolved},{crash.ResolvedAt:yyyy-MM-dd}");
        }

        var fileName = $"Sentinel_Report_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";
        return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", fileName);
    }

    private IQueryable<CrashReport> GetFilteredCrashesQuery(Guid projectId, string? appVersion, string? timePeriod, Severity? severity, string? resolutionStatus, 
        string? searchQuery)
    {
        var query = db.CrashReports
            .Include(c => c.Session)
            .AsNoTracking()
            .AsQueryable();

        // 1. Filter by Project
        query = query.Where(c => c.ProjectId == projectId);

        // 2. Filter by Severity
        if (severity.HasValue)
            query = query.Where(c => c.Severity == severity.Value);

        // 3. Filter by Version
        if (!string.IsNullOrEmpty(appVersion) && appVersion != "All")
            query = query.Where(c => c.Session.AppVersion == appVersion);

        // 4. Filter by Time Period
        if (!string.IsNullOrEmpty(timePeriod))
        {
            var now = DateTime.UtcNow;
            query = timePeriod switch
            {
                "last24h" => query.Where(c => c.Timestamp >= now.AddHours(-24)),
                "last7d" => query.Where(c => c.Timestamp >= now.AddDays(-7)),
                "last30d" => query.Where(c => c.Timestamp >= now.AddDays(-30)),
                _ => query
            };
        }

        // 5. Filter by Resolution Status
        if (!string.IsNullOrEmpty(resolutionStatus))
        {
            query = resolutionStatus switch
            {
                "resolved" => query.Where(c => c.IsResolved),
                "unresolved" => query.Where(c => !c.IsResolved),
                _ => query
            };
        }

        // 6. Search text across multiple fields
        if (!string.IsNullOrEmpty(searchQuery))
        {
            var lowerSearch = searchQuery.ToLower();
            query = query.Where(c =>
                c.ExceptionName.Contains(lowerSearch, StringComparison.CurrentCultureIgnoreCase) ||
                c.Message.Contains(lowerSearch, StringComparison.CurrentCultureIgnoreCase) ||
                c.StackTrace.Contains(lowerSearch, StringComparison.CurrentCultureIgnoreCase) ||
                (c.ResolutionComment != null && c.ResolutionComment.Contains(lowerSearch, StringComparison.CurrentCultureIgnoreCase))
            );
        }

        return query;
    }

    private async Task<ProjectRoleType?> GetUserProjectRole(Guid projectId)
    {
        var userEmail = User.Identity?.Name;
        var membership = await db.ProjectMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserEmail == userEmail && m.IsAccepted);

        return membership?.Role;
    }
}