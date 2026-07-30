using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Entities.Bookings;

public sealed class BookingNoteTests
{
    [Fact]
    public void BookingNote_Should_Assign_BookingNoteId()
    {
        var bookingNote = new BookingNote();
        var id = Guid.NewGuid();
        bookingNote.BookingNoteId = id;
        bookingNote.BookingNoteId.Should().Be(id);
    }

    [Fact]
    public void BookingNote_Should_Assign_BookingId()
    {
        var bookingNote = new BookingNote();
        var bookingId = Guid.NewGuid();
        bookingNote.BookingId = bookingId;
        bookingNote.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void BookingNote_Should_Assign_Note()
    {
        var bookingNote = new BookingNote();
        bookingNote.Note = "Customer requested extra chairs.";
        bookingNote.Note.Should().Be("Customer requested extra chairs.");
    }

    [Fact]
    public void BookingNote_Should_Assign_NoteType()
    {
        var bookingNote = new BookingNote();
        bookingNote.NoteType = "Internal";
        bookingNote.NoteType.Should().Be("Internal");
    }

    [Fact]
    public void BookingNote_Should_Assign_IsImportant()
    {
        var bookingNote = new BookingNote();
        bookingNote.IsImportant = true;
        bookingNote.IsImportant.Should().BeTrue();
    }

    [Fact]
    public void BookingNote_Should_Assign_All_Properties()
    {
        var bookingNoteId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var bookingNote = new BookingNote
        {
            BookingNoteId = bookingNoteId,
            BookingId = bookingId,
            Note = "Customer requested extra chairs.",
            NoteType = "Internal",
            IsImportant = true
        };

        bookingNote.BookingNoteId.Should().Be(bookingNoteId);
        bookingNote.BookingId.Should().Be(bookingId);
        bookingNote.Note.Should().Be("Customer requested extra chairs.");
        bookingNote.NoteType.Should().Be("Internal");
        bookingNote.IsImportant.Should().BeTrue();
    }

    [Fact]
    public void BookingNote_Should_Have_Default_Values()
    {
        var bookingNote = new BookingNote();
        bookingNote.BookingNoteId.Should().Be(Guid.Empty);
        bookingNote.BookingId.Should().Be(Guid.Empty);
        bookingNote.Note.Should().BeEmpty();
        bookingNote.NoteType.Should().BeEmpty();
        bookingNote.IsImportant.Should().BeFalse();
    }
}