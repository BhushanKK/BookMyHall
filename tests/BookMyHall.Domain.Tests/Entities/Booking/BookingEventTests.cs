using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Entities.Bookings;

public sealed class BookingEventTests
{
    [Fact]
    public void BookingEvent_Should_Assign_BookingEventId()
    {
        var bookingEvent = new BookingEvent();
        var id = Guid.NewGuid();
        bookingEvent.BookingEventId = id;
        bookingEvent.BookingEventId.Should().Be(id);
    }

    [Fact]
    public void BookingEvent_Should_Assign_BookingId()
    {
        var bookingEvent = new BookingEvent();
        var bookingId = Guid.NewGuid();
        bookingEvent.BookingId = bookingId;
        bookingEvent.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void BookingEvent_Should_Assign_EventTitle()
    {
        var bookingEvent = new BookingEvent();
        bookingEvent.EventTitle = "Wedding Ceremony";
        bookingEvent.EventTitle.Should().Be("Wedding Ceremony");
    }

    [Fact]
    public void BookingEvent_Should_Assign_HostName()
    {
        var bookingEvent = new BookingEvent();
        bookingEvent.HostName = "Aman Yadav";
        bookingEvent.HostName.Should().Be("Aman Yadav");
    }

    [Fact]
    public void BookingEvent_Should_Assign_BrideName()
    {
        var bookingEvent = new BookingEvent();
        bookingEvent.BrideName = "Tanu";
        bookingEvent.BrideName.Should().Be("Tanu");
    }

    [Fact]
    public void BookingEvent_Should_Assign_GroomName()
    {
        var bookingEvent = new BookingEvent();
        bookingEvent.GroomName = "Dada";
        bookingEvent.GroomName.Should().Be("Dada");
    }

    [Fact]
    public void BookingEvent_Should_Assign_BirthdayPersonName()
    {
        var bookingEvent = new BookingEvent();
        bookingEvent.BirthdayPersonName = "Rahul";
        bookingEvent.BirthdayPersonName.Should().Be("Rahul");
    }

    [Fact]
    public void BookingEvent_Should_Assign_CompanyName()
    {
        var bookingEvent = new BookingEvent();
        bookingEvent.CompanyName = "ABC Pvt Ltd";
        bookingEvent.CompanyName.Should().Be("ABC Pvt Ltd");
    }

    [Fact]
    public void BookingEvent_Should_Assign_Theme()
    {
        var bookingEvent = new BookingEvent();
        bookingEvent.Theme = "Royal";
        bookingEvent.Theme.Should().Be("Royal");
    }

    [Fact]
    public void BookingEvent_Should_Assign_EventDescription()
    {
        var bookingEvent = new BookingEvent();
        bookingEvent.EventDescription = "Traditional wedding ceremony.";
        bookingEvent.EventDescription.Should().Be("Traditional wedding ceremony.");
    }

    [Fact]
    public void BookingEvent_Should_Assign_All_Properties()
    {
        var bookingEventId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var bookingEvent = new BookingEvent
        {
            BookingEventId = bookingEventId,
            BookingId = bookingId,
            EventTitle = "Wedding Ceremony",
            HostName = "Aman Yadav",
            BrideName = "Tanu",
            GroomName = "Dada",
            BirthdayPersonName = "Rahul",
            CompanyName = "ABC Pvt Ltd",
            Theme = "Royal",
            EventDescription = "Traditional wedding ceremony."
        };

        bookingEvent.BookingEventId.Should().Be(bookingEventId);
        bookingEvent.BookingId.Should().Be(bookingId);
        bookingEvent.EventTitle.Should().Be("Wedding Ceremony");
        bookingEvent.HostName.Should().Be("Aman Yadav");
        bookingEvent.BrideName.Should().Be("Tanu");
        bookingEvent.GroomName.Should().Be("Dada");
        bookingEvent.BirthdayPersonName.Should().Be("Rahul");
        bookingEvent.CompanyName.Should().Be("ABC Pvt Ltd");
        bookingEvent.Theme.Should().Be("Royal");
        bookingEvent.EventDescription.Should().Be("Traditional wedding ceremony.");
    }

    [Fact]
    public void BookingEvent_Should_Have_Default_Values()
    {
        var bookingEvent = new BookingEvent();
        bookingEvent.BookingEventId.Should().Be(Guid.Empty);
        bookingEvent.BookingId.Should().Be(Guid.Empty);
        bookingEvent.EventTitle.Should().BeEmpty();
        bookingEvent.HostName.Should().BeEmpty();
        bookingEvent.BrideName.Should().BeEmpty();
        bookingEvent.GroomName.Should().BeEmpty();
        bookingEvent.BirthdayPersonName.Should().BeEmpty();
        bookingEvent.CompanyName.Should().BeEmpty();
        bookingEvent.Theme.Should().BeEmpty();
        bookingEvent.EventDescription.Should().BeEmpty();
    }
}