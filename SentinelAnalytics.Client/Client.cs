using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelAnalytics.Client;

public static class SentinelAnalytics
{
    private static SentinelHttpClient? _client;
    private static SentinelOptions? _options;

    public static void Initialize(SentinelOptions options)
    {
        _options = options;
        _client = new SentinelHttpClient(options);
    }

    public static async Task LogError(
        Exception ex,
        string? userId = null,
        string severity = "Error")
    {
        EnsureInitialized();

        var dto = new CrashReportDto
        {
            SessionId = Guid.NewGuid().ToString(),
            ExceptionName = ex.GetType().Name,
            Message = ex.Message,
            StackTrace = ex.StackTrace ?? string.Empty,
            Severity = severity,
            AppVersion = _options!.AppVersion,
            OsVersion = DeviceInfoProvider.GetOsVersion(),
            DeviceModel = DeviceInfoProvider.GetDeviceModel(),
            UserId = userId
        };

        await _client!.SendCrashAsync(dto);
    }

    public static async Task TrackEvent(
        string eventName,
        object? properties = null,
        string? sessionId = null)
    {
        EnsureInitialized();

        var dto = new MobileEventDto
        {
            EventName = eventName,
            SessionId = sessionId ?? Guid.NewGuid().ToString(),
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