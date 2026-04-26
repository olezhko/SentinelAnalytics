using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using System.Net;
using System.Text;

namespace SentinelAnalytics.Services
{
    public interface ICrashNotificationService
    {
        Task NotifyProjectTeamAsync(Guid projectId, CrashReport crash, CancellationToken cancellationToken);
        Task NotifyProjectTeamAsync(Guid projectId, CrashReport[] crashes, CancellationToken cancellationToken);
        Task NotifyAsync(CancellationToken stoppingToken);
    }

    public class CrashNotificationService(SentinelDbContext db, IEmailSender emailSender) : ICrashNotificationService
    {
        public async Task NotifyProjectTeamAsync(Guid projectId, CrashReport crash, CancellationToken cancellationToken)
        {
            var project = await db.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

            var projectMembers = await db.ProjectMembers
                .AsNoTracking()
                .Include(item => item.User)
                .Where(item => item.ProjectId == projectId && item.User != null)
                .ToListAsync(cancellationToken);

            foreach (var member in projectMembers)
            {
                var userSubscription = await db.UserSubscriptions
                    .Include(item => item.User)
                    .FirstOrDefaultAsync(us => us.UserId == member.UserId && us.User.EmailConfirmed, cancellationToken);

                if (userSubscription != null)
                {
                    await SendCrashesInfoAsync(member.User!, project!.Name, new[] { crash }, cancellationToken);
                }
            }
        }

        public async Task NotifyProjectTeamAsync(Guid projectId, CrashReport[] crashes, CancellationToken cancellationToken)
        {
            var project = await db.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

            var projectMembers = await db.ProjectMembers
                .AsNoTracking()
                .Include(item => item.User)
                .Where(item => item.ProjectId == projectId && item.User != null)
                .ToListAsync(cancellationToken);

            foreach (var member in projectMembers)
            {
                await SendCrashesInfoAsync(member.User!, project.Name, crashes, cancellationToken);
            }
        }

        public async Task NotifyAsync(CancellationToken stoppingToken)
        {
            var startDate = DateTimeOffset.UtcNow.Date - TimeSpan.FromDays(1);

            var crashesToProjectItems = await db.CrashReports
                .Where(item => item.Timestamp.Date >= startDate)
                .AsNoTracking()
                .GroupBy(item => item.ProjectId)
                .ToDictionaryAsync(k => k.Key, v => v.ToList(), stoppingToken);

            foreach (var projectCrashes in crashesToProjectItems)
            {
                await NotifyProjectTeamAsync(projectCrashes.Key, [.. projectCrashes.Value], stoppingToken);
            }
        }

        /// <summary>
        /// This method will be used for sending Realtime, Daily, Weekly crash reports
        /// </summary>
        /// <param name="identityUser"></param>
        /// <param name="projectName"></param>
        /// <param name="crashReports"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SendCrashesInfoAsync(
            IdentityUser identityUser,
            string projectName,
            CrashReport[] crashReports,
            CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();

            sb.Append($@"
                <html>
                <body style='font-family:Arial, Helvetica, sans-serif'>
                <h2>New Crashes Detected</h2>
                <p>Hello <b>{WebUtility.HtmlEncode(identityUser.UserName)}</b>,</p>
                <p>The following crashes were detected in project <b>{WebUtility.HtmlEncode(projectName)}</b>.</p>
        
                <table style='border-collapse: collapse; width:100%;'>
                    <thead>
                        <tr style='background-color:#f2f2f2'>
                            <th style='border:1px solid #ddd;padding:8px'>Timestamp (UTC)</th>
                            <th style='border:1px solid #ddd;padding:8px'>Severity</th>
                            <th style='border:1px solid #ddd;padding:8px'>Exception</th>
                            <th style='border:1px solid #ddd;padding:8px'>Message</th>
                            <th style='border:1px solid #ddd;padding:8px'>User</th>
                            <th style='border:1px solid #ddd;padding:8px'>Session</th>
                            <th style='border:1px solid #ddd;padding:8px'>Resolved</th>
                        </tr>
                    </thead>
                    <tbody>");

            foreach (var crash in crashReports)
            {
                var message = crash.Message.Length > 120
                    ? crash.Message.Substring(0, 120) + "..."
                    : crash.Message;

                sb.Append($@"
                <tr>
                    <td style='border:1px solid #ddd;padding:8px'>{crash.Timestamp:yyyy-MM-dd HH:mm:ss}</td>
                    <td style='border:1px solid #ddd;padding:8px'>{crash.Severity}</td>
                    <td style='border:1px solid #ddd;padding:8px'>{WebUtility.HtmlEncode(crash.ExceptionName)}</td>
                    <td style='border:1px solid #ddd;padding:8px'>{WebUtility.HtmlEncode(message)}</td>
                    <td style='border:1px solid #ddd;padding:8px'>{WebUtility.HtmlEncode(crash.UserId ?? "-")}</td>
                    <td style='border:1px solid #ddd;padding:8px'>{crash.SessionId}</td>
                    <td style='border:1px solid #ddd;padding:8px'>{(crash.IsResolved ? "Yes" : "No")}</td>
                </tr>");
            }

            sb.Append(@"
                    </tbody>
                </table>
        
                <p style='margin-top:20px'>
                    Please visit the <b>Sentinel Analytics dashboard</b> to investigate further.
                </p>

                <p>Best regards,<br/>Sentinel Analytics Team</p>
                </body>
                </html>");

            await emailSender.SendEmailAsync(
                identityUser!.Email!,
                $"New Crashes Detected in {projectName}",
                sb.ToString());
        }
    }
}
