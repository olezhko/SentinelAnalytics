namespace SentinelAnalytics.Maui;

public static class SentinelTracker
{
    private static SentinelHttpClient? _client;

    public static void Initialize(string productKey)
    {
        _client = new SentinelHttpClient(productKey);
    }

    public static async Task TrackErrorAsync(
        Exception ex,
        string sessionId,
        string? userId = null,
        string severity = "Error", 
        IDictionary<string, object> properties = null)
    {
        EnsureInitialized();

        var dto = new CrashReportDto
        {
            SessionId = sessionId,
            ExceptionName = ex.GetType().Name,
            Message = ex.Message,
            StackTrace = ex.StackTrace ?? string.Empty,
            Severity = severity,
            AppVersion = AppInfo.VersionString,
            OsVersion = DeviceInfoProvider.GetOsVersion(),
            DeviceModel = DeviceInfoProvider.GetDeviceModel(),
            UserId = userId,
            Properties = properties
        };

        await _client!.SendCrashAsync(dto);
    }

    public static async Task TrackEventAsync(
        string eventName,
        string sessionId,
        IDictionary<string, object> properties = null)
    {
        EnsureInitialized();

        var dto = new MobileEventDto
        {
            EventName = eventName,
            SessionId = sessionId,
            Properties = properties
        };

        await _client!.SendEventAsync(dto);
    }

    private static void EnsureInitialized()
    {
        if (_client == null)
            throw new InvalidOperationException("SentinelAnalytics.Initialize() was not called.");
    }
}