# Profile managment

https://analytics-mobile.com/

# SentinelAnalytics

Sentinel is the enterprise-grade analytics engine for mobile teams. Track every event, catch every crash, and fix issues faster with AI.

# Last version 1.0.1

# How to Use

    protected override void OnStart()
    {
        SentinelAnalytics.Initialize("prod_key_xxx");
    }
    
    try
    {
        throw new InvalidOperationException("Boom");
    }
    catch (Exception ex)
    {
        await SentinelTracker.TrackErrorAsync(ex, properties: (IDictionary<string, object>)properties, sessionId: SessionId.ToString());
    }
    
    await SentinelTracker.TrackEventAsync(text, properties: (IDictionary<string, object>)properties, sessionId: SessionId.ToString());
