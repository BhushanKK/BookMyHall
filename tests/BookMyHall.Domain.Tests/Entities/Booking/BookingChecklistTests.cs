using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Entities.Bookings;

public sealed class BookingChecklistTests
{
    [Fact]
    public void BookingChecklist_Should_Assign_BookingChecklistId()
    {
        var bookingChecklist = new BookingChecklist();
        var id = Guid.NewGuid();
        bookingChecklist.BookingChecklistId = id;
        bookingChecklist.BookingChecklistId.Should().Be(id);
    }

    [Fact]
    public void BookingChecklist_Should_Assign_BookingId()
    {
        var bookingChecklist = new BookingChecklist();
        var bookingId = Guid.NewGuid();
        bookingChecklist.BookingId = bookingId;
        bookingChecklist.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void BookingChecklist_Should_Assign_ChecklistItem()
    {
        var bookingChecklist = new BookingChecklist();
        bookingChecklist.ChecklistItem = "Verify ID";
        bookingChecklist.ChecklistItem.Should().Be("Verify ID");
    }

    [Fact]
    public void BookingChecklist_Should_Assign_IsCompleted()
    {
        var bookingChecklist = new BookingChecklist();
        bookingChecklist.IsCompleted = true;
        bookingChecklist.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void BookingChecklist_Should_Assign_CompletedBy()
    {
        var bookingChecklist = new BookingChecklist();
        var completedBy = Guid.NewGuid();
        bookingChecklist.CompletedBy = completedBy;
        bookingChecklist.CompletedBy.Should().Be(completedBy);
    }

    [Fact]
    public void BookingChecklist_Should_Assign_CompletedDate()
    {
        var bookingChecklist = new BookingChecklist();
        var completedDate = DateTimeOffset.UtcNow;
        bookingChecklist.CompletedDate = completedDate;
        bookingChecklist.CompletedDate.Should().Be(completedDate);
    }

    [Fact]
    public void BookingChecklist_Should_Assign_Remarks()
    {
        var bookingChecklist = new BookingChecklist();
        bookingChecklist.Remarks = "Completed successfully.";
        bookingChecklist.Remarks.Should().Be("Completed successfully.");
    }

    [Fact]
    public void BookingChecklist_Should_Assign_DisplayOrder()
    {
        var bookingChecklist = new BookingChecklist();
        bookingChecklist.DisplayOrder = 1;
        bookingChecklist.DisplayOrder.Should().Be(1);
    }

    [Fact]
    public void BookingChecklist_Should_Assign_All_Properties()
    {
        var bookingChecklistId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var completedBy = Guid.NewGuid();
        var completedDate = DateTimeOffset.UtcNow;
        var bookingChecklist = new BookingChecklist
        {
            BookingChecklistId = bookingChecklistId,
            BookingId = bookingId,
            ChecklistItem = "Verify ID",
            IsCompleted = true,
            CompletedBy = completedBy,
            CompletedDate = completedDate,
            Remarks = "Completed successfully.",
            DisplayOrder = 1
        };

        bookingChecklist.BookingChecklistId.Should().Be(bookingChecklistId);
        bookingChecklist.BookingId.Should().Be(bookingId);
        bookingChecklist.ChecklistItem.Should().Be("Verify ID");
        bookingChecklist.IsCompleted.Should().BeTrue();
        bookingChecklist.CompletedBy.Should().Be(completedBy);
        bookingChecklist.CompletedDate.Should().Be(completedDate);
        bookingChecklist.Remarks.Should().Be("Completed successfully.");
        bookingChecklist.DisplayOrder.Should().Be(1);
    }

    [Fact]
    public void BookingChecklist_Should_Have_Default_Values()
    {
        var bookingChecklist = new BookingChecklist();
        bookingChecklist.BookingChecklistId.Should().Be(Guid.Empty);
        bookingChecklist.BookingId.Should().Be(Guid.Empty);
        bookingChecklist.ChecklistItem.Should().BeEmpty();
        bookingChecklist.IsCompleted.Should().BeFalse();
        bookingChecklist.CompletedBy.Should().BeNull();
        bookingChecklist.CompletedDate.Should().BeNull();
        bookingChecklist.Remarks.Should().BeEmpty();
        bookingChecklist.DisplayOrder.Should().Be(0);
    }
}