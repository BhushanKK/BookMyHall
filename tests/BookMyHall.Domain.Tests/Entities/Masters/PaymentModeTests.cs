using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Entities.Master;

public sealed class PaymentModeTests
{
    [Fact]
    public void PaymentMode_Should_Be_Inactive_By_Default()
    {
        var paymentMode = new PaymentMode();
        paymentMode.IsActive.Should().BeFalse();
    }

    [Fact]
    public void PaymentMode_Should_Assign_PaymentModeId()
    {
        var paymentMode = new PaymentMode();
        var id = Guid.NewGuid();
        paymentMode.PaymentModeId = id;
        paymentMode.PaymentModeId.Should().Be(id);
    }

    [Fact]
    public void PaymentMode_Should_Assign_PaymentModeName()
    {
        var paymentMode = new PaymentMode();
        paymentMode.PaymentModeName = "Online Payment";
        paymentMode.PaymentModeName.Should().Be("Online Payment");
    }

    [Fact]
    public void PaymentMode_Should_Assign_IsActive()
    {
        var paymentMode = new PaymentMode();
        paymentMode.IsActive = true;
        paymentMode.IsActive.Should().BeTrue();
    }

    [Fact]
    public void PaymentMode_Should_Assign_All_Properties()
    {
        var paymentModeId = Guid.NewGuid();
        var paymentMode = new PaymentMode
        {
            PaymentModeId = paymentModeId,
            PaymentModeName = "Online Payment",
            IsActive = true
        };

        paymentMode.PaymentModeId.Should().Be(paymentModeId);
        paymentMode.PaymentModeName.Should().Be("Online Payment");
        paymentMode.IsActive.Should().BeTrue();
    }

    [Fact]
    public void PaymentMode_Should_Have_Default_Values()
    {
        var paymentMode = new PaymentMode();
        paymentMode.PaymentModeId.Should().Be(Guid.Empty);
        paymentMode.PaymentModeName.Should().BeEmpty();
        paymentMode.IsActive.Should().BeFalse();
    }
}