using System.Net.Http.Json;

namespace SentinelAnalytics.Client;

internal sealed class SentinelHttpClient
{
    private readonly HttpClient _http;

    public SentinelHttpClient(SentinelOptions options)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(options.Endpoint),
            Timeout = options.Timeout
        };

        _http.DefaultRequestHeaders.Add("X-Sentinel-Key", options.ApiKey);
    }

    public async Task SendCrashAsync(CrashReportDto dto)
    {
        var response = await _http.PostAsJsonAsync("crash", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task SendEventAsync(MobileEventDto dto)
    {
        var response = await _http.PostAsJsonAsync("event", dto);
        response.EnsureSuccessStatusCode();
    }
}