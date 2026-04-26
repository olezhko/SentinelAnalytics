using Microsoft.AspNetCore.Identity.UI.Services;
using SentinelAnalytics.Services;

namespace SentinelAnalytics.Extensions
{
    public static class ServiceInstaller
    {
        public static IServiceCollection InstallServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGeminiService, GeminiService>();
            services.AddScoped<IEmailSender, SentinelEmailSender>();
            services.AddScoped<ICrashNotificationService, CrashNotificationService>();

            services.AddHttpClient();

            return services;
        }
    }
}
