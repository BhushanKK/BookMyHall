using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Entities.Bookings;

public sealed class BookingReminderTests
{
    [Fact]
    public void BookingReminder_Should_Assign_BookingReminderId()
    {
        var bookingReminder = new BookingReminder();
        var id = Guid.NewGuid();
        bookingReminder.BookingReminderId = id;
        bookingReminder.BookingReminderId.Should().Be(id);
    }

    [Fact]
    public void BookingReminder_Should_Assign_BookingId()
    {
        var bookingReminder = new BookingReminder();
        var bookingId = Guid.NewGuid();
        bookingReminder.BookingId = bookingId;
        bookingReminder.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void BookingReminder_Should_Assign_ReminderType()
    {
        var bookingReminder = new BookingReminder();
        bookingReminder.ReminderType = "Payment";
        bookingReminder.ReminderType.Should().Be("Payment");
    }

    [Fact]
    public void BookingReminder_Should_Assign_ReminderTitle()
    {
        var bookingReminder = new BookingReminder();
        bookingReminder.ReminderTitle = "Payment Due";
        bookingReminder.ReminderTitle.Should().Be("Payment Due");
    }

    [Fact]
    public void BookingReminder_Should_Assign_ReminderMessage()
    {
        var bookingReminder = new BookingReminder();
        bookingReminder.ReminderMessage = "Your payment is due tomorrow.";
        bookingReminder.ReminderMessage.Should().Be("Your payment is due tomorrow.");
    }

    [Fact]
    public void BookingReminder_Should_Assign_ReminderDate()
    {
        var bookingReminder = new BookingReminder();
        var reminderDate = DateTimeOffset.UtcNow.AddDays(1);
        bookingReminder.ReminderDate = reminderDate;
        bookingReminder.ReminderDate.Should().Be(reminderDate);
    }

    [Fact]
    public void BookingReminder_Should_Assign_NotificationChannel()
    {
        var bookingReminder = new BookingReminder();
        bookingReminder.NotificationChannel = "SMS";
        bookingReminder.NotificationChannel.Should().Be("SMS");
    }

    [Fact]
    public void BookingReminder_Should_Assign_ReminderStatus()
    {
        var bookingReminder = new BookingReminder();
        bookingReminder.ReminderStatus = "Pending";
        bookingReminder.ReminderStatus.Should().Be("Pending");
    }

    [Fact]
    public void BookingReminder_Should_Assign_SentDate()
    {
        var bookingReminder = new BookingReminder();
        var sentDate = DateTimeOffset.UtcNow;
        bookingReminder.SentDate = sentDate;
        bookingReminder.SentDate.Should().Be(sentDate);
    }

    [Fact]
    public void BookingReminder_Should_Assign_All_Properties()
    {
        var bookingReminderId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var reminderDate = DateTimeOffset.UtcNow.AddDays(1);
        var sentDate = DateTimeOffset.UtcNow;
        var bookingReminder = new BookingReminder
        {
            BookingReminderId = bookingReminderId,
            BookingId = bookingId,
            ReminderType = "Payment",
            ReminderTitle = "Payment Due",
            ReminderMessage = "Your payment is due tomorrow.",
            ReminderDate = reminderDate,
            NotificationChannel = "SMS",
            ReminderStatus = "Sent",
            SentDate = sentDate
        };

        bookingReminder.BookingReminderId.Should().Be(bookingReminderId);
        bookingReminder.BookingId.Should().Be(bookingId);
        bookingReminder.ReminderType.Should().Be("Payment");
        bookingReminder.ReminderTitle.Should().Be("Payment Due");
        bookingReminder.ReminderMessage.Should().Be("Your payment is due tomorrow.");
        bookingReminder.ReminderDate.Should().Be(reminderDate);
        bookingReminder.NotificationChannel.Should().Be("SMS");
        bookingReminder.ReminderStatus.Should().Be("Sent");
        bookingReminder.SentDate.Should().Be(sentDate);
    }

    [Fact]
    public void BookingReminder_Should_Have_Default_Values()
    {
        var bookingReminder = new BookingReminder();
        bookingReminder.BookingReminderId.Should().Be(Guid.Empty);
        bookingReminder.BookingId.Should().Be(Guid.Empty);
        bookingReminder.ReminderType.Should().BeEmpty();
        bookingReminder.ReminderTitle.Should().BeEmpty();
        bookingReminder.ReminderMessage.Should().BeEmpty();
        bookingReminder.ReminderDate.Should().Be(default);
        bookingReminder.NotificationChannel.Should().BeEmpty();
        bookingReminder.ReminderStatus.Should().BeEmpty();
        bookingReminder.SentDate.Should().BeNull();
    }
}