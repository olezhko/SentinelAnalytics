namespace SentinelAnalytics.Maui;

public static class SentinelTracker
{
    private static SentinelHttpClient? _client;
    public static string SessionId { get; private set; } = null!;

    public static void Initialize(string productKey)
    {
        _client = new SentinelHttpClient(productKey);
    }

    private static async Task<string> InitSessionAsync() => await _client!.InitSessionAsync();

    public static async Task TrackErrorAsync(
        Exception ex,
        IDictionary<string, object>? properties = null)
    {
        await EnsureInitialized();

        var dto = new CrashReportDto
        {
            SessionId = SessionId,
            ExceptionName = ex.GetType().Name,
            Message = ex.Message,
            StackTrace = ex.StackTrace ?? string.Empty,
            Severity = "Error",
            AppVersion = AppInfo.VersionString,
            OsVersion = DeviceInfoProvider.GetOsVersion(),
            DeviceModel = DeviceInfoProvider.GetDeviceModel(),
            UserId = null,
            Properties = properties
        };

        await _client!.SendCrashAsync(dto);
    }

    public static async Task TrackEventAsync(
        string eventName,
        IDictionary<string, object>? properties = null)
    {
        await EnsureInitialized();

        var dto = new MobileEventDto
        {
            EventName = eventName,
            SessionId = SessionId,
            Properties = properties
        };

        await _client!.SendEventAsync(dto);
    }

    private static async Task EnsureInitialized()
    {
        if (_client == null)
            throw new InvalidOperationException("SentinelTracker.Initialize() was not called.");

        SessionId = await InitSessionAsync();
    }
}