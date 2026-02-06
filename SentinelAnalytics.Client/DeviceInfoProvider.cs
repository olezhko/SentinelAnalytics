using Microsoft.Maui.Devices;

namespace SentinelAnalytics.Client;

internal static class DeviceInfoProvider
{
    public static string GetDeviceModel() =>
        DeviceInfo.Model;

    public static string GetOsVersion() =>
        $"{DeviceInfo.Platform} {DeviceInfo.VersionString}";
}