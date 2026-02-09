using System.Net.Http.Json;

namespace SentinelAnalytics.Maui;

internal sealed class SentinelHttpClient
{
    private readonly HttpClient _http;

    public SentinelHttpClient(string projectApiKey)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri("https://analytics-mobile.com/api/ingest/"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        _http.DefaultRequestHeaders.Add("X-Sentinel-Key", projectApiKey);
    }

    public async Task SendCrashAsync(CrashReportDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("crash", dto);
            response.EnsureSuccessStatusCode();
        }
        catch
        {
        }
    }

    public async Task SendEventAsync(MobileEventDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("event", dto);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
        }
    }
}