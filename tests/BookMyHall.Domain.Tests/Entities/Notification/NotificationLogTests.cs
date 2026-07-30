using FluentAssertions;
using BookMyHall.Domain.Notifications;

namespace BookMyHall.Domain.Tests.Notifications;

public sealed class NotificationLogTests
{
    [Fact]
    public void NotificationLog_Should_Assign_NotificationLogId()
    {
        var notificationLog = new NotificationLog();
        var id = Guid.NewGuid();
        notificationLog.NotificationLogId = id;
        notificationLog.NotificationLogId.Should().Be(id);
    }

    [Fact]
    public void NotificationLog_Should_Assign_NotificationQueueId()
    {
        var notificationLog = new NotificationLog();
        var queueId = Guid.NewGuid();
        notificationLog.NotificationQueueId = queueId;
        notificationLog.NotificationQueueId.Should().Be(queueId);
    }

    [Fact]
    public void NotificationLog_Should_Assign_NotificationId()
    {
        var notificationLog = new NotificationLog();
        var notificationId = Guid.NewGuid();
        notificationLog.NotificationId = notificationId;
        notificationLog.NotificationId.Should().Be(notificationId);
    }

    [Fact]
    public void NotificationLog_Should_Assign_NotificationRecipientId()
    {
        var notificationLog = new NotificationLog();
        var recipientId = Guid.NewGuid();
        notificationLog.NotificationRecipientId = recipientId;
        notificationLog.NotificationRecipientId.Should().Be(recipientId);
    }

    [Fact]
    public void NotificationLog_Should_Assign_NotificationType()
    {
        var notificationLog = new NotificationLog();
        notificationLog.NotificationType = "Email";
        notificationLog.NotificationType.Should().Be("Email");
    }

    [Fact]
    public void NotificationLog_Should_Assign_NotificationProviderId()
    {
        var notificationLog = new NotificationLog();
        var providerId = Guid.NewGuid();
        notificationLog.NotificationProviderId = providerId;
        notificationLog.NotificationProviderId.Should().Be(providerId);
    }

    [Fact]
    public void NotificationLog_Should_Assign_ProviderMessageId()
    {
        var notificationLog = new NotificationLog();
        notificationLog.ProviderMessageId = "MSG123456";
        notificationLog.ProviderMessageId.Should().Be("MSG123456");
    }

    [Fact]
    public void NotificationLog_Should_Assign_Status()
    {
        var notificationLog = new NotificationLog();
        notificationLog.Status = "Delivered";
        notificationLog.Status.Should().Be("Delivered");
    }

    [Fact]
    public void NotificationLog_Should_Assign_ResponseCode()
    {
        var notificationLog = new NotificationLog();
        notificationLog.ResponseCode = "200";
        notificationLog.ResponseCode.Should().Be("200");
    }

    [Fact]
    public void NotificationLog_Should_Assign_ResponseMessage()
    {
        var notificationLog = new NotificationLog();
        notificationLog.ResponseMessage = "Message Delivered Successfully";
        notificationLog.ResponseMessage.Should().Be("Message Delivered Successfully");
    }

    [Fact]
    public void NotificationLog_Should_Assign_SentDate()
    {
        var notificationLog = new NotificationLog();
        var sentDate = DateTimeOffset.UtcNow;
        notificationLog.SentDate = sentDate;
        notificationLog.SentDate.Should().Be(sentDate);
    }

    [Fact]
    public void NotificationLog_Should_Assign_DeliveredDate()
    {
        var notificationLog = new NotificationLog();
        var deliveredDate = DateTimeOffset.UtcNow.AddMinutes(2);
        notificationLog.DeliveredDate = deliveredDate;
        notificationLog.DeliveredDate.Should().Be(deliveredDate);
    }

    [Fact]
    public void NotificationLog_Should_Assign_ErrorMessage()
    {
        var notificationLog = new NotificationLog();
        notificationLog.ErrorMessage = "SMTP connection failed.";
        notificationLog.ErrorMessage.Should().Be("SMTP connection failed.");
    }

    [Fact]
    public void NotificationLog_Should_Assign_All_Properties()
    {
        var notificationLogId = Guid.NewGuid();
        var queueId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var sentDate = DateTimeOffset.UtcNow;
        var deliveredDate = sentDate.AddMinutes(2);

        var notificationLog = new NotificationLog
        {
            NotificationLogId = notificationLogId,
            NotificationQueueId = queueId,
            NotificationId = notificationId,
            NotificationRecipientId = recipientId,
            NotificationType = "Email",
            NotificationProviderId = providerId,
            ProviderMessageId = "MSG123456",
            Status = "Delivered",
            ResponseCode = "200",
            ResponseMessage = "Message Delivered Successfully",
            SentDate = sentDate,
            DeliveredDate = deliveredDate,
            ErrorMessage = string.Empty
        };

        notificationLog.NotificationLogId.Should().Be(notificationLogId);
        notificationLog.NotificationQueueId.Should().Be(queueId);
        notificationLog.NotificationId.Should().Be(notificationId);
        notificationLog.NotificationRecipientId.Should().Be(recipientId);
        notificationLog.NotificationType.Should().Be("Email");
        notificationLog.NotificationProviderId.Should().Be(providerId);
        notificationLog.ProviderMessageId.Should().Be("MSG123456");
        notificationLog.Status.Should().Be("Delivered");
        notificationLog.ResponseCode.Should().Be("200");
        notificationLog.ResponseMessage.Should().Be("Message Delivered Successfully");
        notificationLog.SentDate.Should().Be(sentDate);
        notificationLog.DeliveredDate.Should().Be(deliveredDate);
        notificationLog.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public void NotificationLog_Should_Have_Default_Values()
    {
        var notificationLog = new NotificationLog();

        notificationLog.NotificationLogId.Should().Be(Guid.Empty);
        notificationLog.NotificationQueueId.Should().Be(Guid.Empty);
        notificationLog.NotificationId.Should().Be(Guid.Empty);
        notificationLog.NotificationRecipientId.Should().Be(Guid.Empty);
        notificationLog.NotificationType.Should().BeEmpty();
        notificationLog.NotificationProviderId.Should().Be(Guid.Empty);
        notificationLog.ProviderMessageId.Should().BeEmpty();
        notificationLog.Status.Should().BeEmpty();
        notificationLog.ResponseCode.Should().BeEmpty();
        notificationLog.ResponseMessage.Should().BeEmpty();
        notificationLog.SentDate.Should().BeNull();
        notificationLog.DeliveredDate.Should().BeNull();
        notificationLog.ErrorMessage.Should().BeEmpty();
    }
}