using FluentAssertions;
using BookMyHall.Domain.Payments;

namespace BookMyHall.Domain.Tests.Payments;

public sealed class PaymentWebhookTests
{
    [Fact]
    public void PaymentWebhook_Should_Assign_PaymentWebhookId()
    {
        var webhook = new PaymentWebhook();
        var id = Guid.NewGuid();
        webhook.PaymentWebhookId = id;
        webhook.PaymentWebhookId.Should().Be(id);
    }

    [Fact]
    public void PaymentWebhook_Should_Assign_GatewayName()
    {
        var webhook = new PaymentWebhook();
        webhook.GatewayName = "Razorpay";
        webhook.GatewayName.Should().Be("Razorpay");
    }

    [Fact]
    public void PaymentWebhook_Should_Assign_EventType()
    {
        var webhook = new PaymentWebhook();
        webhook.EventType = "payment.captured";
        webhook.EventType.Should().Be("payment.captured");
    }

    [Fact]
    public void PaymentWebhook_Should_Assign_GatewayEventId()
    {
        var webhook = new PaymentWebhook();
        webhook.GatewayEventId = "evt_123456";
        webhook.GatewayEventId.Should().Be("evt_123456");
    }

    [Fact]
    public void PaymentWebhook_Should_Assign_Payload()
    {
        var webhook = new PaymentWebhook();
        webhook.Payload = "{\"event\":\"payment.captured\"}";
        webhook.Payload.Should().Be("{\"event\":\"payment.captured\"}");
    }

    [Fact]
    public void PaymentWebhook_Should_Assign_Signature()
    {
        var webhook = new PaymentWebhook();
        webhook.Signature = "signature123";
        webhook.Signature.Should().Be("signature123");
    }

    [Fact]
    public void PaymentWebhook_Should_Assign_All_Properties()
    {
        var webhookId = Guid.NewGuid();
        var webhook = new PaymentWebhook
        {
            PaymentWebhookId = webhookId,
            GatewayName = "Razorpay",
            EventType = "payment.captured",
            GatewayEventId = "evt_123456",
            Payload = "{\"event\":\"payment.captured\"}",
            Signature = "signature123"
        };

        webhook.PaymentWebhookId.Should().Be(webhookId);
        webhook.GatewayName.Should().Be("Razorpay");
        webhook.EventType.Should().Be("payment.captured");
        webhook.GatewayEventId.Should().Be("evt_123456");
        webhook.Payload.Should().Be("{\"event\":\"payment.captured\"}");
        webhook.Signature.Should().Be("signature123");
    }

    [Fact]
    public void PaymentWebhook_Should_Have_Default_Values()
    {
        var webhook = new PaymentWebhook();
        webhook.PaymentWebhookId.Should().Be(Guid.Empty);
        webhook.GatewayName.Should().BeEmpty();
        webhook.EventType.Should().BeEmpty();
        webhook.GatewayEventId.Should().BeEmpty();
        webhook.Payload.Should().BeEmpty();
        webhook.Signature.Should().BeEmpty();
    }
}