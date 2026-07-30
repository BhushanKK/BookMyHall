using FluentAssertions;
using BookMyHall.Domain.Payments;

namespace BookMyHall.Domain.Tests.Payments;

public sealed class RefundTests
{
    [Fact]
    public void Refund_Should_Assign_RefundId()
    {
        var refund = new Refund();
        var id = Guid.NewGuid();
        refund.RefundId = id;
        refund.RefundId.Should().Be(id);
    }

    [Fact]
    public void Refund_Should_Assign_PaymentId()
    {
        var refund = new Refund();
        var paymentId = Guid.NewGuid();
        refund.PaymentId = paymentId;
        refund.PaymentId.Should().Be(paymentId);
    }

    [Fact]
    public void Refund_Should_Assign_RefundNumber()
    {
        var refund = new Refund();
        refund.RefundNumber = "REF-2026001";
        refund.RefundNumber.Should().Be("REF-2026001");
    }

    [Fact]
    public void Refund_Should_Assign_RefundDate()
    {
        var refund = new Refund();
        var refundDate = DateTimeOffset.UtcNow;
        refund.RefundDate = refundDate;
        refund.RefundDate.Should().Be(refundDate);
    }

    [Fact]
    public void Refund_Should_Assign_RefundAmount()
    {
        var refund = new Refund();
        refund.RefundAmount = 2500m;
        refund.RefundAmount.Should().Be(2500m);
    }

    [Fact]
    public void Refund_Should_Assign_RefundReason()
    {
        var refund = new Refund();
        refund.RefundReason = "Booking Cancelled";
        refund.RefundReason.Should().Be("Booking Cancelled");
    }

    [Fact]
    public void Refund_Should_Assign_GatewayRefundId()
    {
        var refund = new Refund();
        refund.GatewayRefundId = "GR123456789";
        refund.GatewayRefundId.Should().Be("GR123456789");
    }

    [Fact]
    public void Refund_Should_Assign_RefundStatus()
    {
        var refund = new Refund();
        refund.RefundStatus = "Processed";
        refund.RefundStatus.Should().Be("Processed");
    }

    [Fact]
    public void Refund_Should_Assign_Remarks()
    {
        var refund = new Refund();
        refund.Remarks = "Refund processed successfully.";
        refund.Remarks.Should().Be("Refund processed successfully.");
    }

    [Fact]
    public void Refund_Should_Assign_All_Properties()
    {
        var refundId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var refundDate = DateTimeOffset.UtcNow;
        var refund = new Refund
        {
            RefundId = refundId,
            PaymentId = paymentId,
            RefundNumber = "REF-2026001",
            RefundDate = refundDate,
            RefundAmount = 2500m,
            RefundReason = "Booking Cancelled",
            GatewayRefundId = "GR123456789",
            RefundStatus = "Processed",
            Remarks = "Refund processed successfully."
        };

        refund.RefundId.Should().Be(refundId);
        refund.PaymentId.Should().Be(paymentId);
        refund.RefundNumber.Should().Be("REF-2026001");
        refund.RefundDate.Should().Be(refundDate);
        refund.RefundAmount.Should().Be(2500m);
        refund.RefundReason.Should().Be("Booking Cancelled");
        refund.GatewayRefundId.Should().Be("GR123456789");
        refund.RefundStatus.Should().Be("Processed");
        refund.Remarks.Should().Be("Refund processed successfully.");
    }

    [Fact]
    public void Refund_Should_Have_Default_Values()
    {
        var refund = new Refund();
        refund.RefundId.Should().Be(Guid.Empty);
        refund.PaymentId.Should().Be(Guid.Empty);
        refund.RefundNumber.Should().BeEmpty();
        refund.RefundDate.Should().Be(default);
        refund.RefundAmount.Should().Be(0m);
        refund.RefundReason.Should().BeEmpty();
        refund.GatewayRefundId.Should().BeEmpty();
        refund.RefundStatus.Should().BeEmpty();
        refund.Remarks.Should().BeEmpty();
    }
}