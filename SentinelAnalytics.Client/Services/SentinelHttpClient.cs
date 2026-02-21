using SentinelAnalytics.MAUI.Dto;
using System.Net.Http.Json;

namespace SentinelAnalytics.MAUI.Services;

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

    internal async Task SendCrashAsync(CrashReportDto dto)
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

    internal async Task SendEventAsync(MobileEventDto dto)
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

    internal async Task<string> InitSessionAsync(InitSessionDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("init", dto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception)
        {
            return new Guid().ToString();
        }
    }
}