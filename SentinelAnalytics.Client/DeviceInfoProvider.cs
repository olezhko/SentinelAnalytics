namespace SentinelAnalytics.Maui;

internal static class DeviceInfoProvider
{
    public static string GetDeviceModel() =>
        DeviceInfo.Model;

    public static string GetOsVersion() =>
        $"{DeviceInfo.Platform} {DeviceInfo.VersionString}";
}