using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Entities.Bookings;

public sealed class BookingCancellationTests
{
    [Fact]
    public void BookingCancellation_Should_Assign_BookingCancellationId()
    {
        var bookingCancellation = new BookingCancellation();
        var id = Guid.NewGuid();
        bookingCancellation.BookingCancellationId = id;
        bookingCancellation.BookingCancellationId.Should().Be(id);
    }

    [Fact]
    public void BookingCancellation_Should_Assign_BookingId()
    {
        var bookingCancellation = new BookingCancellation();
        var bookingId = Guid.NewGuid();
        bookingCancellation.BookingId = bookingId;
        bookingCancellation.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void BookingCancellation_Should_Assign_CancellationBy()
    {
        var bookingCancellation = new BookingCancellation();
        var cancellationBy = Guid.NewGuid();
        bookingCancellation.CancellationBy = cancellationBy;
        bookingCancellation.CancellationBy.Should().Be(cancellationBy);
    }

    [Fact]
    public void BookingCancellation_Should_Assign_CancellationDate()
    {
        var bookingCancellation = new BookingCancellation();
        var cancellationDate = DateTimeOffset.UtcNow;
        bookingCancellation.CancellationDate = cancellationDate;
        bookingCancellation.CancellationDate.Should().Be(cancellationDate);
    }

    [Fact]
    public void BookingCancellation_Should_Assign_CancellationReason()
    {
        var bookingCancellation = new BookingCancellation();
        bookingCancellation.CancellationReason = "Customer requested cancellation.";
        bookingCancellation.CancellationReason.Should().Be("Customer requested cancellation.");
    }

    [Fact]
    public void BookingCancellation_Should_Assign_CancellationCharge()
    {
        var bookingCancellation = new BookingCancellation();
        bookingCancellation.CancellationCharge = 1000m;
        bookingCancellation.CancellationCharge.Should().Be(1000m);
    }

    [Fact]
    public void BookingCancellation_Should_Assign_RefundAmount()
    {
        var bookingCancellation = new BookingCancellation();
        bookingCancellation.RefundAmount = 9000m;
        bookingCancellation.RefundAmount.Should().Be(9000m);
    }

    [Fact]
    public void BookingCancellation_Should_Assign_RefundId()
    {
        var bookingCancellation = new BookingCancellation();
        var refundId = Guid.NewGuid();
        bookingCancellation.RefundId = refundId;
        bookingCancellation.RefundId.Should().Be(refundId);
    }

    [Fact]
    public void BookingCancellation_Should_Assign_Remarks()
    {
        var bookingCancellation = new BookingCancellation();
        bookingCancellation.Remarks = "Refund initiated successfully.";
        bookingCancellation.Remarks.Should().Be("Refund initiated successfully.");
    }

    [Fact]
    public void BookingCancellation_Should_Assign_All_Properties()
    {
        var bookingCancellationId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var cancellationBy = Guid.NewGuid();
        var refundId = Guid.NewGuid();
        var cancellationDate = DateTimeOffset.UtcNow;

        var bookingCancellation = new BookingCancellation
        {
            BookingCancellationId = bookingCancellationId,
            BookingId = bookingId,
            CancellationBy = cancellationBy,
            CancellationDate = cancellationDate,
            CancellationReason = "Customer requested cancellation.",
            CancellationCharge = 1000m,
            RefundAmount = 9000m,
            RefundId = refundId,
            Remarks = "Refund initiated successfully."
        };

        bookingCancellation.BookingCancellationId.Should().Be(bookingCancellationId);
        bookingCancellation.BookingId.Should().Be(bookingId);
        bookingCancellation.CancellationBy.Should().Be(cancellationBy);
        bookingCancellation.CancellationDate.Should().Be(cancellationDate);
        bookingCancellation.CancellationReason.Should().Be("Customer requested cancellation.");
        bookingCancellation.CancellationCharge.Should().Be(1000m);
        bookingCancellation.RefundAmount.Should().Be(9000m);
        bookingCancellation.RefundId.Should().Be(refundId);
        bookingCancellation.Remarks.Should().Be("Refund initiated successfully.");
    }

    [Fact]
    public void BookingCancellation_Should_Have_Default_Values()
    {
        var bookingCancellation = new BookingCancellation();

        bookingCancellation.BookingCancellationId.Should().Be(Guid.Empty);
        bookingCancellation.BookingId.Should().Be(Guid.Empty);
        bookingCancellation.CancellationBy.Should().Be(Guid.Empty);
        bookingCancellation.CancellationDate.Should().Be(default);
        bookingCancellation.CancellationReason.Should().BeEmpty();
        bookingCancellation.CancellationCharge.Should().Be(0m);
        bookingCancellation.RefundAmount.Should().Be(0m);
        bookingCancellation.RefundId.Should().Be(Guid.Empty);
        bookingCancellation.Remarks.Should().BeEmpty();
    }
}