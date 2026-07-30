using FluentAssertions;
using BookMyHall.Domain.Payments;

namespace BookMyHall.Domain.Tests.Payments;

public sealed class PaymentTests
{
    [Fact]
    public void Payment_Should_Assign_PaymentId()
    {
        var payment = new Payment();
        var id = Guid.NewGuid();

        payment.PaymentId = id;

        payment.PaymentId.Should().Be(id);
    }

    [Fact]
    public void Payment_Should_Assign_BookingId()
    {
        var payment = new Payment();
        var bookingId = Guid.NewGuid();

        payment.BookingId = bookingId;

        payment.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void Payment_Should_Assign_PaymentNumber()
    {
        var payment = new Payment();

        payment.PaymentNumber = "PAY-2026001";

        payment.PaymentNumber.Should().Be("PAY-2026001");
    }

    [Fact]
    public void Payment_Should_Assign_PaymentDate()
    {
        var payment = new Payment();
        var paymentDate = DateTimeOffset.UtcNow;

        payment.PaymentDate = paymentDate;

        payment.PaymentDate.Should().Be(paymentDate);
    }

    [Fact]
    public void Payment_Should_Assign_PaymentModeId()
    {
        var payment = new Payment();
        var paymentModeId = Guid.NewGuid();

        payment.PaymentModeId = paymentModeId;

        payment.PaymentModeId.Should().Be(paymentModeId);
    }

    [Fact]
    public void Payment_Should_Assign_Amount()
    {
        var payment = new Payment();

        payment.Amount = 10000m;

        payment.Amount.Should().Be(10000m);
    }

    [Fact]
    public void Payment_Should_Assign_GatewayTransactionId()
    {
        var payment = new Payment();

        payment.GatewayTransactionId = "TXN123456789";

        payment.GatewayTransactionId.Should().Be("TXN123456789");
    }

    [Fact]
    public void Payment_Should_Assign_GatewayReferenceNumber()
    {
        var payment = new Payment();

        payment.GatewayReferenceNumber = "REF987654321";

        payment.GatewayReferenceNumber.Should().Be("REF987654321");
    }

    [Fact]
    public void Payment_Should_Assign_PaymentStatus()
    {
        var payment = new Payment();

        payment.PaymentStatus = "Success";

        payment.PaymentStatus.Should().Be("Success");
    }

    [Fact]
    public void Payment_Should_Assign_Remarks()
    {
        var payment = new Payment();

        payment.Remarks = "Payment received successfully.";

        payment.Remarks.Should().Be("Payment received successfully.");
    }

    [Fact]
    public void Payment_Should_Assign_All_Properties()
    {
        var paymentId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var paymentModeId = Guid.NewGuid();
        var paymentDate = DateTimeOffset.UtcNow;

        var payment = new Payment
        {
            PaymentId = paymentId,
            BookingId = bookingId,
            PaymentNumber = "PAY-2026001",
            PaymentDate = paymentDate,
            PaymentModeId = paymentModeId,
            Amount = 10000m,
            GatewayTransactionId = "TXN123456789",
            GatewayReferenceNumber = "REF987654321",
            PaymentStatus = "Success",
            Remarks = "Payment received successfully."
        };

        payment.PaymentId.Should().Be(paymentId);
        payment.BookingId.Should().Be(bookingId);
        payment.PaymentNumber.Should().Be("PAY-2026001");
        payment.PaymentDate.Should().Be(paymentDate);
        payment.PaymentModeId.Should().Be(paymentModeId);
        payment.Amount.Should().Be(10000m);
        payment.GatewayTransactionId.Should().Be("TXN123456789");
        payment.GatewayReferenceNumber.Should().Be("REF987654321");
        payment.PaymentStatus.Should().Be("Success");
        payment.Remarks.Should().Be("Payment received successfully.");
    }

    [Fact]
    public void Payment_Should_Have_Default_Values()
    {
        var payment = new Payment();

        payment.PaymentId.Should().Be(Guid.Empty);
        payment.BookingId.Should().Be(Guid.Empty);
        payment.PaymentNumber.Should().BeEmpty();
        payment.PaymentDate.Should().Be(default);
        payment.PaymentModeId.Should().Be(Guid.Empty);
        payment.Amount.Should().Be(0m);
        payment.GatewayTransactionId.Should().BeEmpty();
        payment.GatewayReferenceNumber.Should().BeEmpty();
        payment.PaymentStatus.Should().BeEmpty();
        payment.Remarks.Should().BeEmpty();
    }
}