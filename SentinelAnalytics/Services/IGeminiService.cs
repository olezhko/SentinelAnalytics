using SentinelAnalytics.Data.Entities;
using System.Text;
using System.Text.Json;

namespace SentinelAnalytics.Services;

public interface IGeminiService
{
    Task<string> AnalyzeCrashAsync(CrashReport crash);
}

public class GeminiService(HttpClient httpClient, IConfiguration configuration) : IGeminiService
{
    private readonly string _apiKey = configuration["Gemini:ApiKey"] 
        ?? Environment.GetEnvironmentVariable("API_KEY");

    public async Task<string> AnalyzeCrashAsync(CrashReport crash)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

        var prompt = $@"Analyze this mobile crash and suggest a fix.
                Exception: {crash.ExceptionName}
                Message: {crash.Message}
                Stack: {crash.StackTrace}
                Device: {crash.Session.DeviceModel} ({crash.Session.OsVersion})";

        var requestBody = new
        {
            contents = new[] {
                    new { parts = new[] { new { text = prompt } } }
                }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("candidates")[0]
                .GetProperty("content").GetProperty("parts")[0]
                .GetProperty("text").GetString();
        }

        return "AI Analysis failed to generate.";
    }
}