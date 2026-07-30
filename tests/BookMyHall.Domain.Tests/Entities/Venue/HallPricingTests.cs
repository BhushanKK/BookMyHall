using FluentAssertions;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Domain.Tests.Venue;

public sealed class HallPricingTests
{
    [Fact]
    public void HallPricing_Should_Be_Inactive_By_Default()
    {
        var pricing = new HallPricing();
        pricing.IsActive.Should().BeFalse();
    }

    [Fact]
    public void HallPricing_Should_Assign_HallPricingId()
    {
        var pricing = new HallPricing();
        var id = Guid.NewGuid();
        pricing.HallPricingId = id;
        pricing.HallPricingId.Should().Be(id);
    }

    [Fact]
    public void HallPricing_Should_Assign_HallId()
    {
        var pricing = new HallPricing();
        var hallId = Guid.NewGuid();
        pricing.HallId = hallId;
        pricing.HallId.Should().Be(hallId);
    }

    [Fact]
    public void HallPricing_Should_Assign_EventCategoryId()
    {
        var pricing = new HallPricing();
        var eventCategoryId = Guid.NewGuid();
        pricing.EventCategoryId = eventCategoryId;
        pricing.EventCategoryId.Should().Be(eventCategoryId);
    }

    [Fact]
    public void HallPricing_Should_Assign_PackageName()
    {
        var pricing = new HallPricing();
        pricing.PackageName = "Premium Package";
        pricing.PackageName.Should().Be("Premium Package");
    }

    [Fact]
    public void HallPricing_Should_Assign_MinimumGuests()
    {
        var pricing = new HallPricing();
        pricing.MinimumGuests = "100";
        pricing.MinimumGuests.Should().Be("100");
    }

    [Fact]
    public void HallPricing_Should_Assign_MaximumGuests()
    {
        var pricing = new HallPricing();
        pricing.MaximumGuests = "500";
        pricing.MaximumGuests.Should().Be("500");
    }

    [Fact]
    public void HallPricing_Should_Assign_WeekdayPrice()
    {
        var pricing = new HallPricing();
        pricing.WeekdayPrice = 50000;
        pricing.WeekdayPrice.Should().Be(50000);
    }

    [Fact]
    public void HallPricing_Should_Assign_WeekendPrice()
    {
        var pricing = new HallPricing();
        pricing.WeekendPrice = 75000;
        pricing.WeekendPrice.Should().Be(75000);
    }

    [Fact]
    public void HallPricing_Should_Assign_AdvanceAmount()
    {
        var pricing = new HallPricing();
        pricing.AdvanceAmount = 20000;
        pricing.AdvanceAmount.Should().Be(20000);
    }

    [Fact]
    public void HallPricing_Should_Assign_SecurityDeposit()
    {
        var pricing = new HallPricing();
        pricing.SecurityDeposit = 10000;
        pricing.SecurityDeposit.Should().Be(10000);
    }

    [Fact]
    public void HallPricing_Should_Assign_ExtraGuestCharge()
    {
        var pricing = new HallPricing();
        pricing.ExtraGuestCharge = 500;
        pricing.ExtraGuestCharge.Should().Be(500);
    }

    [Fact]
    public void HallPricing_Should_Assign_IsActive()
    {
        var pricing = new HallPricing();
        pricing.IsActive = true;
        pricing.IsActive.Should().BeTrue();
    }

    [Fact]
    public void HallPricing_Should_Assign_All_Properties()
    {
        var hallPricingId = Guid.NewGuid();
        var hallId = Guid.NewGuid();
        var eventCategoryId = Guid.NewGuid();
        var pricing = new HallPricing
        {
            HallPricingId = hallPricingId,
            HallId = hallId,
            EventCategoryId = eventCategoryId,
            PackageName = "Premium Package",
            MinimumGuests = "100",
            MaximumGuests = "500",
            WeekdayPrice = 50000,
            WeekendPrice = 75000,
            AdvanceAmount = 20000,
            SecurityDeposit = 10000,
            ExtraGuestCharge = 500,
            IsActive = true
        };

        pricing.HallPricingId.Should().Be(hallPricingId);
        pricing.HallId.Should().Be(hallId);
        pricing.EventCategoryId.Should().Be(eventCategoryId);
        pricing.PackageName.Should().Be("Premium Package");
        pricing.MinimumGuests.Should().Be("100");
        pricing.MaximumGuests.Should().Be("500");
        pricing.WeekdayPrice.Should().Be(50000);
        pricing.WeekendPrice.Should().Be(75000);
        pricing.AdvanceAmount.Should().Be(20000);
        pricing.SecurityDeposit.Should().Be(10000);
        pricing.ExtraGuestCharge.Should().Be(500);
        pricing.IsActive.Should().BeTrue();
    }

    [Fact]
    public void HallPricing_Should_Have_Default_Values()
    {
        var pricing = new HallPricing();

        pricing.HallPricingId.Should().Be(Guid.Empty);
        pricing.HallId.Should().Be(Guid.Empty);
        pricing.EventCategoryId.Should().Be(Guid.Empty);
        pricing.PackageName.Should().BeEmpty();
        pricing.MinimumGuests.Should().BeEmpty();
        pricing.MaximumGuests.Should().BeEmpty();
        pricing.WeekdayPrice.Should().Be(0);
        pricing.WeekendPrice.Should().Be(0);
        pricing.AdvanceAmount.Should().Be(0);
        pricing.SecurityDeposit.Should().Be(0);
        pricing.ExtraGuestCharge.Should().Be(0);
        pricing.IsActive.Should().BeFalse();
    }
}