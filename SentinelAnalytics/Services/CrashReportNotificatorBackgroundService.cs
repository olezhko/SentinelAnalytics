namespace SentinelAnalytics.Services
{
    public class CrashReportNotificatorBackgroundService(
        ILogger<CrashReportNotificatorBackgroundService> logger,
        ICrashNotificationService crashNotificationService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("CrashReportNotificatorBackgroundService is starting.");

            while (stoppingToken.IsCancellationRequested)
            {
                var period = TimeSpan.FromDays(1);

                await crashNotificationService.NotifyAsync(stoppingToken);

                await Task.Delay(period, stoppingToken);
            }

            logger.LogInformation("CrashReportNotificatorBackgroundService has completed sending notifications.");
        }
    }
}
