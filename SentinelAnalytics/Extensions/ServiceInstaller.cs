using Microsoft.AspNetCore.Identity.UI.Services;
using SentinelAnalytics.Services;
using Stripe;

namespace SentinelAnalytics.Extensions
{
    public static class ServiceInstaller
    {
        public static IServiceCollection InstallServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGeminiService, GeminiService>();
            services.AddScoped<IEmailSender, SentinelEmailSender>();
            services.AddScoped<ICrashNotificationService, CrashNotificationService>();
            services.AddScoped<IStripeService, StripeService>();

            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            services.AddHttpClient();

            return services;
        }
    }
}
