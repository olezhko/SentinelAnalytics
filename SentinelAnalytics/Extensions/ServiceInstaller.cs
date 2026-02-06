using SentinelAnalytics.Services;

namespace SentinelAnalytics.Extensions
{
    public static class ServiceInstaller
    {
        public static IServiceCollection InstallServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGeminiService, GeminiService>();

            services.AddHttpClient();

            return services;
        }
    }
}
