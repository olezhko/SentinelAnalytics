using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using Stripe;

namespace SentinelAnalytics.Controllers;

[ApiController]
[Route("api/stripe/webhook")]
public class StripeWebhookController(SentinelDbContext db, IConfiguration configuration, ILogger<StripeWebhookController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var webhookSecret = configuration["Stripe:WebhookSecret"];
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                webhookSecret);
        }
        catch (StripeException ex)
        {
            logger.LogWarning("Stripe webhook signature verification failed: {Message}", ex.Message);
            return BadRequest();
        }

        switch (stripeEvent.Type)
        {
            case EventTypes.CustomerSubscriptionUpdated:
                await HandleSubscriptionUpdated((Subscription)stripeEvent.Data.Object);
                break;

            case EventTypes.CustomerSubscriptionDeleted:
                await HandleSubscriptionDeleted((Subscription)stripeEvent.Data.Object);
                break;

            case EventTypes.InvoicePaymentFailed:
                await HandlePaymentFailed((Invoice)stripeEvent.Data.Object);
                break;
        }

        return Ok();
    }

    private async Task HandleSubscriptionUpdated(Subscription subscription)
    {
        var detail = await db.UserDetails
            .FirstOrDefaultAsync(d => d.StripeSubscriptionId == subscription.Id);

        if (detail == null) return;

        detail.StripeSubscriptionStatus = subscription.Status;
        await db.SaveChangesAsync();
    }

    private async Task HandleSubscriptionDeleted(Subscription subscription)
    {
        var detail = await db.UserDetails
            .FirstOrDefaultAsync(d => d.StripeSubscriptionId == subscription.Id);

        if (detail == null) return;

        var freePlan = await db.PricingPlans.FirstOrDefaultAsync(p => p.Price == 0);
        if (freePlan != null) detail.PlanId = freePlan.Id;

        detail.StripeSubscriptionId = null;
        detail.StripeSubscriptionStatus = "canceled";
        await db.SaveChangesAsync();
    }

    private async Task HandlePaymentFailed(Invoice invoice)
    {
        var detail = await db.UserDetails
            .FirstOrDefaultAsync(d => d.StripeCustomerId == invoice.CustomerId);

        if (detail == null) return;

        detail.StripeSubscriptionStatus = "past_due";
        await db.SaveChangesAsync();
    }
}
