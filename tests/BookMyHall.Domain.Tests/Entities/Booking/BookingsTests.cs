using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Booking;

public sealed class BookingsTests
{
    [Fact]
    public void Bookings_Should_Assign_BookingId()
    {
        var booking = new Bookings();
        var id = Guid.NewGuid();

        booking.BookingId = id;

        booking.BookingId.Should().Be(id);
    }

    [Fact]
    public void Bookings_Should_Assign_BookingNumber()
    {
        var booking = new Bookings();

        booking.BookingNumber = "BK-2026001";

        booking.BookingNumber.Should().Be("BK-2026001");
    }

    [Fact]
    public void Bookings_Should_Assign_HallId()
    {
        var booking = new Bookings();
        var hallId = Guid.NewGuid();

        booking.HallId = hallId;

        booking.HallId.Should().Be(hallId);
    }

    [Fact]
    public void Bookings_Should_Assign_CustomerId()
    {
        var booking = new Bookings();
        var customerId = Guid.NewGuid();

        booking.CustomerId = customerId;

        booking.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public void Bookings_Should_Assign_EventCategoryId()
    {
        var booking = new Bookings();
        var eventCategoryId = Guid.NewGuid();

        booking.EventCategoryId = eventCategoryId;

        booking.EventCategoryId.Should().Be(eventCategoryId);
    }

    [Fact]
    public void Bookings_Should_Assign_HallPricingId()
    {
        var booking = new Bookings();
        var hallPricingId = Guid.NewGuid();

        booking.HallPricingId = hallPricingId;

        booking.HallPricingId.Should().Be(hallPricingId);
    }

    [Fact]
    public void Bookings_Should_Assign_BookingDate()
    {
        var booking = new Bookings();
        var date = DateTimeOffset.UtcNow;

        booking.BookingDate = date;

        booking.BookingDate.Should().Be(date);
    }

    [Fact]
    public void Bookings_Should_Assign_StartTime()
    {
        var booking = new Bookings();
        var time = new TimeSpan(10, 0, 0);

        booking.StartTime = time;

        booking.StartTime.Should().Be(time);
    }

    [Fact]
    public void Bookings_Should_Assign_EndTime()
    {
        var booking = new Bookings();
        var time = new TimeSpan(22, 0, 0);

        booking.EndTime = time;

        booking.EndTime.Should().Be(time);
    }

    [Fact]
    public void Bookings_Should_Assign_GuestCount()
    {
        var booking = new Bookings();

        booking.GuestCount = 250;

        booking.GuestCount.Should().Be(250);
    }

    [Fact]
    public void Bookings_Should_Assign_BaseAmount()
    {
        var booking = new Bookings();

        booking.BaseAmount = 50000;

        booking.BaseAmount.Should().Be(50000);
    }

    [Fact]
    public void Bookings_Should_Assign_DiscountAmount()
    {
        var booking = new Bookings();

        booking.DiscountAmount = 5000;

        booking.DiscountAmount.Should().Be(5000);
    }

    [Fact]
    public void Bookings_Should_Assign_TaxAmount()
    {
        var booking = new Bookings();

        booking.TaxAmount = 9000;

        booking.TaxAmount.Should().Be(9000);
    }

    [Fact]
    public void Bookings_Should_Assign_TotalAmount()
    {
        var booking = new Bookings();

        booking.TotalAmount = 54000;

        booking.TotalAmount.Should().Be(54000);
    }

    [Fact]
    public void Bookings_Should_Assign_PaidAmount()
    {
        var booking = new Bookings();

        booking.PaidAmount = 20000;

        booking.PaidAmount.Should().Be(20000);
    }

    [Fact]
    public void Bookings_Should_Assign_BalanceAmount()
    {
        var booking = new Bookings();

        booking.BalanceAmount = 34000;

        booking.BalanceAmount.Should().Be(34000);
    }

    [Fact]
    public void Bookings_Should_Assign_SpecialRequests()
    {
        var booking = new Bookings();

        booking.SpecialRequests = "Need decoration setup";

        booking.SpecialRequests.Should().Be("Need decoration setup");
    }

    [Fact]
    public void Bookings_Should_Assign_BookingStatus()
    {
        var booking = new Bookings();

        booking.BookingStatus = "Confirmed";

        booking.BookingStatus.Should().Be("Confirmed");
    }

    [Fact]
    public void Bookings_Should_Assign_All_Properties()
    {
        var bookingId = Guid.NewGuid();
        var hallId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var eventCategoryId = Guid.NewGuid();
        var hallPricingId = Guid.NewGuid();
        var bookingDate = DateTimeOffset.UtcNow;

        var booking = new Bookings
        {
            BookingId = bookingId,
            BookingNumber = "BK-2026001",
            HallId = hallId,
            CustomerId = customerId,
            EventCategoryId = eventCategoryId,
            HallPricingId = hallPricingId,
            BookingDate = bookingDate,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(22, 0, 0),
            GuestCount = 250,
            BaseAmount = 50000,
            DiscountAmount = 5000,
            TaxAmount = 9000,
            TotalAmount = 54000,
            PaidAmount = 20000,
            BalanceAmount = 34000,
            SpecialRequests = "Need decoration setup",
            BookingStatus = "Confirmed"
        };

        booking.BookingId.Should().Be(bookingId);
        booking.BookingNumber.Should().Be("BK-2026001");
        booking.HallId.Should().Be(hallId);
        booking.CustomerId.Should().Be(customerId);
        booking.EventCategoryId.Should().Be(eventCategoryId);
        booking.HallPricingId.Should().Be(hallPricingId);
        booking.BookingDate.Should().Be(bookingDate);
        booking.StartTime.Should().Be(new TimeSpan(10, 0, 0));
        booking.EndTime.Should().Be(new TimeSpan(22, 0, 0));
        booking.GuestCount.Should().Be(250);
        booking.BaseAmount.Should().Be(50000);
        booking.DiscountAmount.Should().Be(5000);
        booking.TaxAmount.Should().Be(9000);
        booking.TotalAmount.Should().Be(54000);
        booking.PaidAmount.Should().Be(20000);
        booking.BalanceAmount.Should().Be(34000);
        booking.SpecialRequests.Should().Be("Need decoration setup");
        booking.BookingStatus.Should().Be("Confirmed");
    }

    [Fact]
    public void Bookings_Should_Have_Default_Values()
    {
        var booking = new Bookings();

        booking.BookingId.Should().Be(Guid.Empty);
        booking.BookingNumber.Should().BeEmpty();
        booking.HallId.Should().Be(Guid.Empty);
        booking.CustomerId.Should().Be(Guid.Empty);
        booking.EventCategoryId.Should().Be(Guid.Empty);
        booking.HallPricingId.Should().Be(Guid.Empty);
        booking.BookingDate.Should().Be(default);
        booking.StartTime.Should().Be(default);
        booking.EndTime.Should().Be(default);
        booking.GuestCount.Should().Be(0);
        booking.BaseAmount.Should().Be(0);
        booking.DiscountAmount.Should().Be(0);
        booking.TaxAmount.Should().Be(0);
        booking.TotalAmount.Should().Be(0);
        booking.PaidAmount.Should().Be(0);
        booking.BalanceAmount.Should().Be(0);
        booking.SpecialRequests.Should().BeEmpty();
        booking.BookingStatus.Should().BeEmpty();
    }
}