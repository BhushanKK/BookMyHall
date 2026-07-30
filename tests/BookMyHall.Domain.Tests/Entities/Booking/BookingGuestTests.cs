using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Entities.Bookings;

public sealed class BookingGuestTests
{
    [Fact]
    public void BookingGuest_Should_Assign_BookingGuestId()
    {
        var bookingGuest = new BookingGuest();
        var id = Guid.NewGuid();
        bookingGuest.BookingGuestId = id;
        bookingGuest.BookingGuestId.Should().Be(id);
    }

    [Fact]
    public void BookingGuest_Should_Assign_BookingId()
    {
        var bookingGuest = new BookingGuest();
        var bookingId = Guid.NewGuid();
        bookingGuest.BookingId = bookingId;
        bookingGuest.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void BookingGuest_Should_Assign_GuestName()
    {
        var bookingGuest = new BookingGuest();
        bookingGuest.GuestName = "Aman Yadav";
        bookingGuest.GuestName.Should().Be("Aman Yadav");
    }

    [Fact]
    public void BookingGuest_Should_Assign_EmailAddress()
    {
        var bookingGuest = new BookingGuest();
        bookingGuest.EmailAddress = "Aman@example.com";
        bookingGuest.EmailAddress.Should().Be("Aman@example.com");
    }

    [Fact]
    public void BookingGuest_Should_Assign_MobileNumber()
    {
        var bookingGuest = new BookingGuest();
        bookingGuest.MobileNumber = "9876543210";
        bookingGuest.MobileNumber.Should().Be("9876543210");
    }

    [Fact]
    public void BookingGuest_Should_Assign_Address()
    {
        var bookingGuest = new BookingGuest();
        bookingGuest.Address = "123 Main Street";
        bookingGuest.Address.Should().Be("123 Main Street");
    }

    [Fact]
    public void BookingGuest_Should_Assign_City()
    {
        var bookingGuest = new BookingGuest();
        bookingGuest.City = "Mumbai";
        bookingGuest.City.Should().Be("Mumbai");
    }

    [Fact]
    public void BookingGuest_Should_Assign_State()
    {
        var bookingGuest = new BookingGuest();
        bookingGuest.State = "Maharashtra";
        bookingGuest.State.Should().Be("Maharashtra");
    }

    [Fact]
    public void BookingGuest_Should_Assign_Pincode()
    {
        var bookingGuest = new BookingGuest();
        bookingGuest.Pincode = "400001";
        bookingGuest.Pincode.Should().Be("400001");
    }

    [Fact]
    public void BookingGuest_Should_Assign_All_Properties()
    {
        var bookingGuestId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var bookingGuest = new BookingGuest
        {
            BookingGuestId = bookingGuestId,
            BookingId = bookingId,
            GuestName = "Aman Yadav",
            EmailAddress = "Aman@example.com",
            MobileNumber = "9876543210",
            Address = "123 Main Street",
            City = "Mumbai",
            State = "Maharashtra",
            Pincode = "400001"
        };

        bookingGuest.BookingGuestId.Should().Be(bookingGuestId);
        bookingGuest.BookingId.Should().Be(bookingId);
        bookingGuest.GuestName.Should().Be("Aman Yadav");
        bookingGuest.EmailAddress.Should().Be("Aman@example.com");
        bookingGuest.MobileNumber.Should().Be("9876543210");
        bookingGuest.Address.Should().Be("123 Main Street");
        bookingGuest.City.Should().Be("Mumbai");
        bookingGuest.State.Should().Be("Maharashtra");
        bookingGuest.Pincode.Should().Be("400001");
    }

    [Fact]
    public void BookingGuest_Should_Have_Default_Values()
    {
        var bookingGuest = new BookingGuest();
        bookingGuest.BookingGuestId.Should().Be(Guid.Empty);
        bookingGuest.BookingId.Should().Be(Guid.Empty);
        bookingGuest.GuestName.Should().BeEmpty();
        bookingGuest.EmailAddress.Should().BeEmpty();
        bookingGuest.MobileNumber.Should().BeEmpty();
        bookingGuest.Address.Should().BeEmpty();
        bookingGuest.City.Should().BeEmpty();
        bookingGuest.State.Should().BeEmpty();
        bookingGuest.Pincode.Should().BeEmpty();
    }
}