using FluentAssertions;
using BookMyHall.Domain.Payments;

namespace BookMyHall.Domain.Tests.Payments;

public sealed class PaymentTransactionTests
{
    [Fact]
    public void PaymentTransaction_Should_Assign_PaymentTransactionId()
    {
        var transaction = new PaymentTransaction();
        var id = Guid.NewGuid();
        transaction.PaymentTransactionId = id;
        transaction.PaymentTransactionId.Should().Be(id);
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_PaymentId()
    {
        var transaction = new PaymentTransaction();
        var paymentId = Guid.NewGuid();
        transaction.PaymentId = paymentId;
        transaction.PaymentId.Should().Be(paymentId);
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_TransactionNumber()
    {
        var transaction = new PaymentTransaction();
        transaction.TransactionNumber = "TXN-2026001";
        transaction.TransactionNumber.Should().Be("TXN-2026001");
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_GatewayName()
    {
        var transaction = new PaymentTransaction();
        transaction.GatewayName = "Razorpay";
        transaction.GatewayName.Should().Be("Razorpay");
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_GatewayTransactionId()
    {
        var transaction = new PaymentTransaction();
        transaction.GatewayTransactionId = "GTX123456";
        transaction.GatewayTransactionId.Should().Be("GTX123456");
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_GatewayOrderId()
    {
        var transaction = new PaymentTransaction();
        transaction.GatewayOrderId = "ORD123456";
        transaction.GatewayOrderId.Should().Be("ORD123456");
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_GatewayPaymentId()
    {
        var transaction = new PaymentTransaction();
        transaction.GatewayPaymentId = "PAY123456";
        transaction.GatewayPaymentId.Should().Be("PAY123456");
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_TransactionAmount()
    {
        var transaction = new PaymentTransaction();
        transaction.TransactionAmount = 5000m;
        transaction.TransactionAmount.Should().Be(5000m);
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_CurrencyCode()
    {
        var transaction = new PaymentTransaction();
        transaction.CurrencyCode = "INR";
        transaction.CurrencyCode.Should().Be("INR");
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_TransactionStatus()
    {
        var transaction = new PaymentTransaction();
        transaction.TransactionStatus = "Success";
        transaction.TransactionStatus.Should().Be("Success");
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_GatewayResponse()
    {
        var transaction = new PaymentTransaction();
        transaction.GatewayResponse = "{\"status\":\"success\"}";
        transaction.GatewayResponse.Should().Be("{\"status\":\"success\"}");
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_TransactionDate()
    {
        var transaction = new PaymentTransaction();
        var transactionDate = DateTimeOffset.UtcNow;
        transaction.TransactionDate = transactionDate;
        transaction.TransactionDate.Should().Be(transactionDate);
    }

    [Fact]
    public void PaymentTransaction_Should_Assign_All_Properties()
    {
        var transactionId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var transactionDate = DateTimeOffset.UtcNow;
        var transaction = new PaymentTransaction
        {
            PaymentTransactionId = transactionId,
            PaymentId = paymentId,
            TransactionNumber = "TXN-2026001",
            GatewayName = "Razorpay",
            GatewayTransactionId = "GTX123456",
            GatewayOrderId = "ORD123456",
            GatewayPaymentId = "PAY123456",
            TransactionAmount = 5000m,
            CurrencyCode = "INR",
            TransactionStatus = "Success",
            GatewayResponse = "{\"status\":\"success\"}",
            TransactionDate = transactionDate
        };

        transaction.PaymentTransactionId.Should().Be(transactionId);
        transaction.PaymentId.Should().Be(paymentId);
        transaction.TransactionNumber.Should().Be("TXN-2026001");
        transaction.GatewayName.Should().Be("Razorpay");
        transaction.GatewayTransactionId.Should().Be("GTX123456");
        transaction.GatewayOrderId.Should().Be("ORD123456");
        transaction.GatewayPaymentId.Should().Be("PAY123456");
        transaction.TransactionAmount.Should().Be(5000m);
        transaction.CurrencyCode.Should().Be("INR");
        transaction.TransactionStatus.Should().Be("Success");
        transaction.GatewayResponse.Should().Be("{\"status\":\"success\"}");
        transaction.TransactionDate.Should().Be(transactionDate);
    }

    [Fact]
    public void PaymentTransaction_Should_Have_Default_Values()
    {
        var transaction = new PaymentTransaction();

        transaction.PaymentTransactionId.Should().Be(Guid.Empty);
        transaction.PaymentId.Should().Be(Guid.Empty);
        transaction.TransactionNumber.Should().BeEmpty();
        transaction.GatewayName.Should().BeEmpty();
        transaction.GatewayTransactionId.Should().BeEmpty();
        transaction.GatewayOrderId.Should().BeEmpty();
        transaction.GatewayPaymentId.Should().BeEmpty();
        transaction.TransactionAmount.Should().Be(0m);
        transaction.CurrencyCode.Should().BeEmpty();
        transaction.TransactionStatus.Should().BeEmpty();
        transaction.GatewayResponse.Should().BeEmpty();
        transaction.TransactionDate.Should().Be(default);
    }
}