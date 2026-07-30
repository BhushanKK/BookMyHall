using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Entities.Bookings;

public sealed class BookingTimelineTests
{
    [Fact]
    public void BookingTimeline_Should_Assign_BookingTimelineId()
    {
        var bookingTimeline = new BookingTimeline();
        var id = Guid.NewGuid();
        bookingTimeline.BookingTimelineId = id;
        bookingTimeline.BookingTimelineId.Should().Be(id);
    }

    [Fact]
    public void BookingTimeline_Should_Assign_BookingId()
    {
        var bookingTimeline = new BookingTimeline();
        var bookingId = Guid.NewGuid();
        bookingTimeline.BookingId = bookingId;
        bookingTimeline.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void BookingTimeline_Should_Assign_ActivityType()
    {
        var bookingTimeline = new BookingTimeline();
        bookingTimeline.ActivityType = "Booking";
        bookingTimeline.ActivityType.Should().Be("Booking");
    }

    [Fact]
    public void BookingTimeline_Should_Assign_ActivityTitle()
    {
        var bookingTimeline = new BookingTimeline();
        bookingTimeline.ActivityTitle = "Booking Created";
        bookingTimeline.ActivityTitle.Should().Be("Booking Created");
    }

    [Fact]
    public void BookingTimeline_Should_Assign_ActivityDescription()
    {
        var bookingTimeline = new BookingTimeline();
        bookingTimeline.ActivityDescription = "Booking was successfully created.";
        bookingTimeline.ActivityDescription.Should().Be("Booking was successfully created.");
    }

    [Fact]
    public void BookingTimeline_Should_Assign_PerformedBy()
    {
        var bookingTimeline = new BookingTimeline();
        var performedBy = Guid.NewGuid();
        bookingTimeline.PerformedBy = performedBy;
        bookingTimeline.PerformedBy.Should().Be(performedBy);
    }

    [Fact]
    public void BookingTimeline_Should_Assign_PerformedByType()
    {
        var bookingTimeline = new BookingTimeline();
        bookingTimeline.PerformedByType = "Admin";
        bookingTimeline.PerformedByType.Should().Be("Admin");
    }

    [Fact]
    public void BookingTimeline_Should_Assign_ReferenceId()
    {
        var bookingTimeline = new BookingTimeline();
        var referenceId = Guid.NewGuid();
        bookingTimeline.ReferenceId = referenceId;
        bookingTimeline.ReferenceId.Should().Be(referenceId);
    }

    [Fact]
    public void BookingTimeline_Should_Assign_ReferenceType()
    {
        var bookingTimeline = new BookingTimeline();
        bookingTimeline.ReferenceType = "Payment";
        bookingTimeline.ReferenceType.Should().Be("Payment");
    }

    [Fact]
    public void BookingTimeline_Should_Assign_ActivityDate()
    {
        var bookingTimeline = new BookingTimeline();
        var activityDate = DateTimeOffset.UtcNow;
        bookingTimeline.ActivityDate = activityDate;
        bookingTimeline.ActivityDate.Should().Be(activityDate);
    }

    [Fact]
    public void BookingTimeline_Should_Assign_All_Properties()
    {
        var bookingTimelineId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var performedBy = Guid.NewGuid();
        var referenceId = Guid.NewGuid();
        var activityDate = DateTimeOffset.UtcNow;
        var bookingTimeline = new BookingTimeline
        {
            BookingTimelineId = bookingTimelineId,
            BookingId = bookingId,
            ActivityType = "Booking",
            ActivityTitle = "Booking Created",
            ActivityDescription = "Booking was successfully created.",
            PerformedBy = performedBy,
            PerformedByType = "Admin",
            ReferenceId = referenceId,
            ReferenceType = "Payment",
            ActivityDate = activityDate
        };

        bookingTimeline.BookingTimelineId.Should().Be(bookingTimelineId);
        bookingTimeline.BookingId.Should().Be(bookingId);
        bookingTimeline.ActivityType.Should().Be("Booking");
        bookingTimeline.ActivityTitle.Should().Be("Booking Created");
        bookingTimeline.ActivityDescription.Should().Be("Booking was successfully created.");
        bookingTimeline.PerformedBy.Should().Be(performedBy);
        bookingTimeline.PerformedByType.Should().Be("Admin");
        bookingTimeline.ReferenceId.Should().Be(referenceId);
        bookingTimeline.ReferenceType.Should().Be("Payment");
        bookingTimeline.ActivityDate.Should().Be(activityDate);
    }

    [Fact]
    public void BookingTimeline_Should_Have_Default_Values()
    {
        var bookingTimeline = new BookingTimeline();
        bookingTimeline.BookingTimelineId.Should().Be(Guid.Empty);
        bookingTimeline.BookingId.Should().Be(Guid.Empty);
        bookingTimeline.ActivityType.Should().BeEmpty();
        bookingTimeline.ActivityTitle.Should().BeEmpty();
        bookingTimeline.ActivityDescription.Should().BeEmpty();
        bookingTimeline.PerformedBy.Should().Be(Guid.Empty);
        bookingTimeline.PerformedByType.Should().BeEmpty();
        bookingTimeline.ReferenceId.Should().Be(Guid.Empty);
        bookingTimeline.ReferenceType.Should().BeEmpty();
        bookingTimeline.ActivityDate.Should().Be(default);
    }
}