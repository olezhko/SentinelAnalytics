# SentinelAnalytics

Sentinel is the enterprise-grade analytics engine for mobile teams. Track every event, catch every crash, and fix issues faster with AI.

# How to Use

    protected override void OnStart()
    {
        SentinelAnalytics.Initialize(new SentinelOptions
        {
            ApiKey = "prod_key_xxx",
            Endpoint = "https://api.yourdomain.com/api/ingest",
            AppVersion = AppInfo.VersionString
        });
    }
    
    try
    {
        throw new InvalidOperationException("Boom");
    }
    catch (Exception ex)
    {
        await SentinelAnalytics.LogError(ex, userId: "42");
    }
    
    await SentinelAnalytics.TrackEvent("purchase_completed", new
    {
        Price = 9.99,
        Currency = "USD"
    });
