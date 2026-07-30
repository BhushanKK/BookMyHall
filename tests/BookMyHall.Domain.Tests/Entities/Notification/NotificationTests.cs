using FluentAssertions;
using BookMyHall.Domain.Notifications;

namespace BookMyHall.Domain.Tests.Notifications;

public sealed class NotificationTests
{
    [Fact]
    public void Notification_Should_Assign_NotificationId()
    {
        var notification = new Notification();
        var id = Guid.NewGuid();
        notification.NotificationId = id;
        notification.NotificationId.Should().Be(id);
    }

    [Fact]
    public void Notification_Should_Assign_Title()
    {
        var notification = new Notification();
        notification.Title = "Booking Confirmed";
        notification.Title.Should().Be("Booking Confirmed");
    }

    [Fact]
    public void Notification_Should_Assign_Message()
    {
        var notification = new Notification();
        notification.Message = "Your booking has been confirmed.";
        notification.Message.Should().Be("Your booking has been confirmed.");
    }

    [Fact]
    public void Notification_Should_Assign_ReferenceId()
    {
        var notification = new Notification();
        var referenceId = Guid.NewGuid();
        notification.ReferenceId = referenceId;
        notification.ReferenceId.Should().Be(referenceId);
    }

    [Fact]
    public void Notification_Should_Assign_NotificationType()
    {
        var notification = new Notification();
        notification.NotificationType = "Email";
        notification.NotificationType.Should().Be("Email");
    }

    [Fact]
    public void Notification_Should_Assign_ModuleName()
    {
        var notification = new Notification();
        notification.ModuleName = "Booking";
        notification.ModuleName.Should().Be("Booking");
    }

    [Fact]
    public void Notification_Should_Assign_Priority()
    {
        var notification = new Notification();
        notification.Priority = "High";
        notification.Priority.Should().Be("High");
    }

    [Fact]
    public void Notification_Should_Assign_Status()
    {
        var notification = new Notification();
        notification.Status = "Pending";
        notification.Status.Should().Be("Pending");
    }

    [Fact]
    public void Notification_Should_Assign_ScheduledDate()
    {
        var notification = new Notification();
        var scheduledDate = DateTimeOffset.UtcNow.AddHours(1);
        notification.ScheduledDate = scheduledDate;
        notification.ScheduledDate.Should().Be(scheduledDate);
    }

    [Fact]
    public void Notification_Should_Assign_All_Properties()
    {
        var notificationId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();
        var scheduledDate = DateTimeOffset.UtcNow.AddHours(1);
        var notification = new Notification
        {
            NotificationId = notificationId,
            Title = "Booking Confirmed",
            Message = "Your booking has been confirmed.",
            ReferenceId = referenceId,
            NotificationType = "Email",
            ModuleName = "Booking",
            Priority = "High",
            Status = "Pending",
            ScheduledDate = scheduledDate
        };

        notification.NotificationId.Should().Be(notificationId);
        notification.Title.Should().Be("Booking Confirmed");
        notification.Message.Should().Be("Your booking has been confirmed.");
        notification.ReferenceId.Should().Be(referenceId);
        notification.NotificationType.Should().Be("Email");
        notification.ModuleName.Should().Be("Booking");
        notification.Priority.Should().Be("High");
        notification.Status.Should().Be("Pending");
        notification.ScheduledDate.Should().Be(scheduledDate);
    }

    [Fact]
    public void Notification_Should_Have_Default_Values()
    {
        var notification = new Notification();
        notification.NotificationId.Should().Be(Guid.Empty);
        notification.Title.Should().BeEmpty();
        notification.Message.Should().BeEmpty();
        notification.ReferenceId.Should().Be(Guid.Empty);
        notification.NotificationType.Should().BeEmpty();
        notification.ModuleName.Should().BeEmpty();
        notification.Priority.Should().BeEmpty();
        notification.Status.Should().BeEmpty();
        notification.ScheduledDate.Should().Be(default);
    }
}