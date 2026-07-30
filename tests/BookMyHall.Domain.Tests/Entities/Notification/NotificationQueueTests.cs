using FluentAssertions;
using BookMyHall.Domain.Notifications;

namespace BookMyHall.Domain.Tests.Notifications;

public sealed class NotificationQueueTests
{
    [Fact]
    public void NotificationQueue_Should_Assign_NotificationQueueId()
    {
        var queue = new NotificationQueue();
        var id = Guid.NewGuid();
        queue.NotificationQueueId = id;
        queue.NotificationQueueId.Should().Be(id);
    }

    [Fact]
    public void NotificationQueue_Should_Assign_NotificationId()
    {
        var queue = new NotificationQueue();
        var notificationId = Guid.NewGuid();
        queue.NotificationId = notificationId;
        queue.NotificationId.Should().Be(notificationId);
    }

    [Fact]
    public void NotificationQueue_Should_Assign_NotificationRecipientId()
    {
        var queue = new NotificationQueue();
        var recipientId = Guid.NewGuid();
        queue.NotificationRecipientId = recipientId;
        queue.NotificationRecipientId.Should().Be(recipientId);
    }

    [Fact]
    public void NotificationQueue_Should_Assign_NotificationTemplateId()
    {
        var queue = new NotificationQueue();
        var templateId = Guid.NewGuid();
        queue.NotificationTemplateId = templateId;
        queue.NotificationTemplateId.Should().Be(templateId);
    }

    [Fact]
    public void NotificationQueue_Should_Assign_ScheduledDate()
    {
        var queue = new NotificationQueue();
        var scheduledDate = DateTimeOffset.UtcNow;
        queue.ScheduledDate = scheduledDate;
        queue.ScheduledDate.Should().Be(scheduledDate);
    }

    [Fact]
    public void NotificationQueue_Should_Assign_Status()
    {
        var queue = new NotificationQueue();
        queue.Status = "Pending";
        queue.Status.Should().Be("Pending");
    }

    [Fact]
    public void NotificationQueue_Should_Assign_RetryCount()
    {
        var queue = new NotificationQueue();
        queue.RetryCount = 2;
        queue.RetryCount.Should().Be(2);
    }

    [Fact]
    public void NotificationQueue_Should_Assign_MaxRetryCount()
    {
        var queue = new NotificationQueue();
        queue.MaxRetryCount = 5;
        queue.MaxRetryCount.Should().Be(5);
    }

    [Fact]
    public void NotificationQueue_Should_Assign_LastAttemptDate()
    {
        var queue = new NotificationQueue();
        var lastAttemptDate = DateTimeOffset.UtcNow;
        queue.LastAttemptDate = lastAttemptDate;
        queue.LastAttemptDate.Should().Be(lastAttemptDate);
    }

    [Fact]
    public void NotificationQueue_Should_Assign_ProcessedDate()
    {
        var queue = new NotificationQueue();
        var processedDate = DateTimeOffset.UtcNow;
        queue.ProcessedDate = processedDate;
        queue.ProcessedDate.Should().Be(processedDate);
    }

    [Fact]
    public void NotificationQueue_Should_Assign_ErrorMessage()
    {
        var queue = new NotificationQueue();
        queue.ErrorMessage = "SMTP server unavailable.";
        queue.ErrorMessage.Should().Be("SMTP server unavailable.");
    }

    [Fact]
    public void NotificationQueue_Should_Assign_All_Properties()
    {
        var queueId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var templateId = Guid.NewGuid();
        var scheduledDate = DateTimeOffset.UtcNow;
        var lastAttemptDate = scheduledDate.AddMinutes(5);
        var processedDate = scheduledDate.AddMinutes(10);

        var queue = new NotificationQueue
        {
            NotificationQueueId = queueId,
            NotificationId = notificationId,
            NotificationRecipientId = recipientId,
            NotificationTemplateId = templateId,
            ScheduledDate = scheduledDate,
            Status = "Processed",
            RetryCount = 1,
            MaxRetryCount = 3,
            LastAttemptDate = lastAttemptDate,
            ProcessedDate = processedDate,
            ErrorMessage = string.Empty
        };

        queue.NotificationQueueId.Should().Be(queueId);
        queue.NotificationId.Should().Be(notificationId);
        queue.NotificationRecipientId.Should().Be(recipientId);
        queue.NotificationTemplateId.Should().Be(templateId);
        queue.ScheduledDate.Should().Be(scheduledDate);
        queue.Status.Should().Be("Processed");
        queue.RetryCount.Should().Be(1);
        queue.MaxRetryCount.Should().Be(3);
        queue.LastAttemptDate.Should().Be(lastAttemptDate);
        queue.ProcessedDate.Should().Be(processedDate);
        queue.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public void NotificationQueue_Should_Have_Default_Values()
    {
        var queue = new NotificationQueue();
        queue.NotificationQueueId.Should().Be(Guid.Empty);
        queue.NotificationId.Should().Be(Guid.Empty);
        queue.NotificationRecipientId.Should().Be(Guid.Empty);
        queue.NotificationTemplateId.Should().Be(Guid.Empty);
        queue.ScheduledDate.Should().Be(default);
        queue.Status.Should().BeEmpty();
        queue.RetryCount.Should().Be(0);
        queue.MaxRetryCount.Should().Be(0);
        queue.LastAttemptDate.Should().BeNull();
        queue.ProcessedDate.Should().BeNull();
        queue.ErrorMessage.Should().BeEmpty();
    }
}