using Stripe;

namespace SentinelAnalytics.Services;

public record StripeSubscriptionResult(string SubscriptionId, string Status);
public record StripePaymentMethodDetails(string LastFour, string Brand, string Expiry, string? CardholderName);

public interface IStripeService
{
    Task<string> GetOrCreateCustomerAsync(string userId, string email);
    Task<StripeSubscriptionResult> SubscribeAsync(string customerId, string stripePriceId, string paymentMethodId);
    Task<StripeSubscriptionResult> UpdateSubscriptionPlanAsync(string subscriptionId, string newPriceId);
    Task UpdatePaymentMethodAsync(string customerId, string subscriptionId, string paymentMethodId);
    Task CancelSubscriptionAsync(string subscriptionId);
    Task<StripePaymentMethodDetails> GetPaymentMethodDetailsAsync(string paymentMethodId);
}

public class StripeService : IStripeService
{
    private readonly CustomerService _customers = new();
    private readonly SubscriptionService _subscriptions = new();
    private readonly PaymentMethodService _paymentMethods = new();

    public async Task<string> GetOrCreateCustomerAsync(string userId, string email)
    {
        var existing = await _customers.ListAsync(new CustomerListOptions
        {
            Email = email,
            Limit = 1,
        });

        if (existing.Data.Count > 0)
            return existing.Data[0].Id;

        var customer = await _customers.CreateAsync(new CustomerCreateOptions
        {
            Email = email,
            Metadata = new Dictionary<string, string> { ["userId"] = userId },
        });

        return customer.Id;
    }

    public async Task<StripeSubscriptionResult> SubscribeAsync(string customerId, string stripePriceId, string paymentMethodId)
    {
        await _paymentMethods.AttachAsync(paymentMethodId, new PaymentMethodAttachOptions
        {
            Customer = customerId,
        });

        await _customers.UpdateAsync(customerId, new CustomerUpdateOptions
        {
            InvoiceSettings = new CustomerInvoiceSettingsOptions
            {
                DefaultPaymentMethod = paymentMethodId,
            },
        });

        var subscription = await _subscriptions.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = customerId,
            Items = [new SubscriptionItemOptions { Price = stripePriceId }],
            DefaultPaymentMethod = paymentMethodId,
        });

        return new StripeSubscriptionResult(subscription.Id, subscription.Status);
    }

    public async Task<StripeSubscriptionResult> UpdateSubscriptionPlanAsync(string subscriptionId, string newPriceId)
    {
        var subscription = await _subscriptions.GetAsync(subscriptionId);
        var itemId = subscription.Items.Data[0].Id;

        var updated = await _subscriptions.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
        {
            Items =
            [
                new SubscriptionItemOptions { Id = itemId, Price = newPriceId },
            ],
        });

        return new StripeSubscriptionResult(updated.Id, updated.Status);
    }

    public async Task UpdatePaymentMethodAsync(string customerId, string subscriptionId, string paymentMethodId)
    {
        await _paymentMethods.AttachAsync(paymentMethodId, new PaymentMethodAttachOptions
        {
            Customer = customerId,
        });

        await _subscriptions.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
        {
            DefaultPaymentMethod = paymentMethodId,
        });

        await _customers.UpdateAsync(customerId, new CustomerUpdateOptions
        {
            InvoiceSettings = new CustomerInvoiceSettingsOptions
            {
                DefaultPaymentMethod = paymentMethodId,
            },
        });
    }

    public async Task CancelSubscriptionAsync(string subscriptionId)
    {
        await _subscriptions.CancelAsync(subscriptionId);
    }

    public async Task<StripePaymentMethodDetails> GetPaymentMethodDetailsAsync(string paymentMethodId)
    {
        var pm = await _paymentMethods.GetAsync(paymentMethodId);
        var card = pm.Card;
        var expiry = $"{card.ExpMonth:D2}/{card.ExpYear % 100:D2}";
        return new StripePaymentMethodDetails(card.Last4, Capitalize(card.Brand), expiry, pm.BillingDetails?.Name);
    }

    private static string Capitalize(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];
}
