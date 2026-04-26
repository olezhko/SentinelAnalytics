using SentinelAnalytics.MAUI.Dto;
using SentinelAnalytics.MAUI.Exceptions;
using SentinelAnalytics.MAUI.Services;

namespace SentinelAnalytics.Maui;

public static class SentinelTracker
{
    private static bool IsIgnoreEmulators { get; set; }
    private static SentinelHttpClient? _client;

    public static string SessionId { get; private set; } = null!;

    /// <summary>
    /// Generates a test crash by throwing a <see cref="TestCrashException"/>. This method is intended for testing purposes to simulate a crash scenario and verify that the tracking system correctly captures and reports the exception details. When called, it will immediately throw the specified exception, allowing developers to observe how the tracking client handles and logs the error information.
    /// </summary>
    /// <exception cref="TestCrashException"></exception>
    public static void GenerateTestCrash()
    {
        throw new TestCrashException();
    }

    /// <summary>
    /// Initializes the Sentinel client with the specified product key and optional emulator setting.
    /// </summary>
    /// <remarks>This method must be called before using any other Sentinel client methods to ensure proper
    /// initialization.</remarks>
    /// <param name="productKey">The product key used to authenticate the Sentinel client.</param>
    /// <param name="isIgnoreEmulators">Indicates whether to ignore emulator instances. If <see langword="true"/>, the client will not connect to
    /// emulators.</param>
    public static void Initialize(string productKey, bool isIgnoreEmulators = false)
    {
        _client = new SentinelHttpClient(productKey);
        IsIgnoreEmulators = isIgnoreEmulators;

        SubscribeCustomEvents();
    }

    private static void SubscribeCustomEvents()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
            Exception ex = args.ExceptionObject as Exception;
            _ = Task.Run(() => TrackErrorAsync(ex, severity: Severity.Critical));
            Thread.Sleep(200);
        };

        TaskScheduler.UnobservedTaskException += (sender, args) => {
            _ = Task.Run(() => TrackErrorAsync(args.Exception, severity: Severity.Critical));
            args.SetObserved();
            Thread.Sleep(200);
        };

#if ANDROID
        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) => {
            _ = Task.Run(() => TrackErrorAsync(args.Exception, severity: Severity.Critical));
            Thread.Sleep(200);
        };
#endif

#if IOS
        AppDomain.CurrentDomain.UnhandledException += (s, e) => {
            Exception ex = e.ExceptionObject as Exception;
            _ = Task.Run(() => TrackErrorAsync(ex, severity: Severity.Critical));
            Thread.Sleep(200);
        };
#endif
    }

    /// <summary>
    /// Tracks an error synchronously by logging the specified exception and optional contextual properties.
    /// </summary>
    /// <remarks>This method initiates error tracking in a background task, allowing the calling thread to
    /// continue without waiting for completion. It is suitable for scenarios where error logging should not block
    /// application execution.</remarks>
    /// <param name="ex">The exception to be tracked. Provides information about the error that occurred.</param>
    /// <param name="properties">An optional dictionary containing additional properties to associate with the error. Can be used to supply
    /// contextual information or metadata.</param>
    public static void TrackError(
        Exception ex,
        Severity severity,
        IDictionary<string, object>? properties = null)
    {
        _ = Task.Run(() => TrackErrorAsync(ex, severity, properties));
    }

    /// <summary>
    /// Tracks an event synchronously by its name, optionally including additional properties for context.
    /// </summary>
    /// <remarks>This method runs the tracking operation in a separate task, allowing the caller to continue
    /// execution without waiting for the tracking to complete.</remarks>
    /// <param name="eventName">The name of the event to track. This should be a non-empty string that identifies the event uniquely.</param>
    /// <param name="properties">An optional dictionary of additional properties associated with the event. Each key-value pair provides context
    /// about the event being tracked.</param>
    public static void TrackEvent(
        string eventName,
        IDictionary<string, object>? properties = null)
    {
        _ = Task.Run(() => TrackEventAsync(eventName, properties));
    }

    /// <summary>
    /// Asynchronously tracks an error by sending a crash report containing the specified exception details and optional
    /// contextual properties.
    /// </summary>
    /// <remarks>The tracking client is initialized before the crash report is sent. The crash report includes
    /// the exception type, message, stack trace, and any additional properties provided. This method does not throw if
    /// the client is uninitialized; initialization is handled internally.</remarks>
    /// <param name="ex">The exception to be reported. Cannot be null. Provides information about the error, including its type, message,
    /// and stack trace.</param>
    /// <param name="properties">An optional dictionary of additional properties to include in the crash report. Can be used to supply contextual
    /// information relevant to the error.</param>
    /// <returns>A task that represents the asynchronous operation of sending the crash report.</returns>
    public static async Task TrackErrorAsync(
        Exception ex,
        Severity severity,
        IDictionary<string, object>? properties = null)
    {
        await EnsureInitialized();

        await SendErrorAsync(ex, severity, properties: properties);
    }

    /// <summary>
    /// Tracks an event asynchronously by sending the event data to a remote server.
    /// </summary>
    /// <remarks>This method ensures that the tracking client is initialized before sending the event. It is
    /// important to call this method after the client has been properly set up.</remarks>
    /// <param name="eventName">The name of the event to track. This value cannot be null or empty.</param>
    /// <param name="properties">An optional dictionary of additional properties associated with the event. Each property should be a key-value
    /// pair where the key is a string and the value is an object.</param>
    /// <returns>A task that represents the asynchronous operation of tracking the event.</returns>
    public static async Task TrackEventAsync(
        string eventName,
        IDictionary<string, object>? properties = null)
    {
        await EnsureInitialized();

        await SendEventAsync(eventName, properties);
    }

    private static async Task EnsureInitialized()
    {
        if (IsIgnoreEmulators && IsRunningOnEmulator())
        {
            return;
        }

        if (_client == null)
            throw new InvalidOperationException("SentinelTracker.Initialize() was not called.");
        
        if (SessionId != null)
            return;

        SessionId = await InitSessionAsync();
    }

    private static async Task<string> InitSessionAsync()
    {
        var dto = new InitSessionDto
        {
            DeviceId = DeviceInfoProvider.GetDeviceId(),
            Country = DeviceInfoProvider.GetCountry(),
            Language = DeviceInfoProvider.GetLanguage(),
            AppVersion = AppInfo.VersionString,
            OsVersion = DeviceInfoProvider.GetOsVersion(),
            DeviceModel = DeviceInfoProvider.GetDeviceModel(),
        };

        return await _client!.InitSessionAsync(dto);
    }

    private static async Task SendErrorAsync(Exception ex, Severity severity = Severity.Error, IDictionary<string, object>? properties = null)
    {
        var dto = new CrashReportDto
        {
            SessionId = SessionId,
            ExceptionName = ex.GetType().Name,
            Message = ex.Message,
            StackTrace = ex.StackTrace ?? string.Empty,
            Severity = severity,
            UserId = null,
            Properties = properties
        };

        await _client!.SendCrashAsync(dto);
    }

    private static async Task SendEventAsync(string eventName, IDictionary<string, object>? properties)
    {
        var dto = new MobileEventDto
        {
            EventName = eventName,
            SessionId = SessionId,
            Properties = properties
        };

        await _client!.SendEventAsync(dto);
    }

    private static bool IsRunningOnEmulator() => DeviceInfo.DeviceType != DeviceType.Physical;
}