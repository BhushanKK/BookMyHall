using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Entities.Bookings;

public sealed class BookingStatusHistoryTests
{
    [Fact]
    public void BookingStatusHistory_Should_Assign_BookingStatusHistoryId()
    {
        var bookingStatusHistory = new BookingStatusHistory();
        var id = Guid.NewGuid();
        bookingStatusHistory.BookingStatusHistoryId = id;
        bookingStatusHistory.BookingStatusHistoryId.Should().Be(id);
    }

    [Fact]
    public void BookingStatusHistory_Should_Assign_BookingId()
    {
        var bookingStatusHistory = new BookingStatusHistory();
        var bookingId = Guid.NewGuid();
        bookingStatusHistory.BookingId = bookingId;
        bookingStatusHistory.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void BookingStatusHistory_Should_Assign_OldStatus()
    {
        var bookingStatusHistory = new BookingStatusHistory();
        bookingStatusHistory.OldStatus = "Pending";
        bookingStatusHistory.OldStatus.Should().Be("Pending");
    }

    [Fact]
    public void BookingStatusHistory_Should_Assign_NewStatus()
    {
        var bookingStatusHistory = new BookingStatusHistory();
        bookingStatusHistory.NewStatus = "Confirmed";
        bookingStatusHistory.NewStatus.Should().Be("Confirmed");
    }

    [Fact]
    public void BookingStatusHistory_Should_Assign_Remarks()
    {
        var bookingStatusHistory = new BookingStatusHistory();
        bookingStatusHistory.Remarks = "Booking confirmed by admin.";
        bookingStatusHistory.Remarks.Should().Be("Booking confirmed by admin.");
    }

    [Fact]
    public void BookingStatusHistory_Should_Assign_ChangeBy()
    {
        var bookingStatusHistory = new BookingStatusHistory();
        var changedBy = Guid.NewGuid();
        bookingStatusHistory.ChangeBy = changedBy;
        bookingStatusHistory.ChangeBy.Should().Be(changedBy);
    }

    [Fact]
    public void BookingStatusHistory_Should_Assign_All_Properties()
    {
        var bookingStatusHistoryId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var changedBy = Guid.NewGuid();
        var bookingStatusHistory = new BookingStatusHistory
        {
            BookingStatusHistoryId = bookingStatusHistoryId,
            BookingId = bookingId,
            OldStatus = "Pending",
            NewStatus = "Confirmed",
            Remarks = "Booking confirmed by admin.",
            ChangeBy = changedBy
        };

        bookingStatusHistory.BookingStatusHistoryId.Should().Be(bookingStatusHistoryId);
        bookingStatusHistory.BookingId.Should().Be(bookingId);
        bookingStatusHistory.OldStatus.Should().Be("Pending");
        bookingStatusHistory.NewStatus.Should().Be("Confirmed");
        bookingStatusHistory.Remarks.Should().Be("Booking confirmed by admin.");
        bookingStatusHistory.ChangeBy.Should().Be(changedBy);
    }

    [Fact]
    public void BookingStatusHistory_Should_Have_Default_Values()
    {
        var bookingStatusHistory = new BookingStatusHistory();
        bookingStatusHistory.BookingStatusHistoryId.Should().Be(Guid.Empty);
        bookingStatusHistory.BookingId.Should().Be(Guid.Empty);
        bookingStatusHistory.OldStatus.Should().BeEmpty();
        bookingStatusHistory.NewStatus.Should().BeEmpty();
        bookingStatusHistory.Remarks.Should().BeEmpty();
        bookingStatusHistory.ChangeBy.Should().Be(Guid.Empty);
    }
}