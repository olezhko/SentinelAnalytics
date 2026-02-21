namespace SentinelAnalytics.MAUI.Services;

internal static class DeviceInfoProvider
{
    internal static string GetDeviceModel() =>
        DeviceInfo.Model;

    internal static string GetOsVersion() =>
        $"{DeviceInfo.Platform} {DeviceInfo.VersionString}";

    internal static string GetCountry()
    {
        return System.Globalization.RegionInfo.CurrentRegion.DisplayName;
    }

    internal static string GetDeviceId()
    {
        string deviceID = Guid.NewGuid().ToString();

        try
        {
#if ANDROID
            deviceID = Android.Provider.Settings.Secure.GetString(Platform.CurrentActivity.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
#elif IOS
            deviceID = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();
#endif
        }
        catch (Exception)
        {
        }

        return deviceID;
    }

    internal static string GetLanguage()
    {
        return System.Globalization.CultureInfo.CurrentUICulture.Name;
    }
}