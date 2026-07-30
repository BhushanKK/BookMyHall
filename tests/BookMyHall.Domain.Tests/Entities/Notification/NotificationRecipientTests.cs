using FluentAssertions;
using BookMyHall.Domain.Notifications;

namespace BookMyHall.Domain.Tests.Notifications;

public sealed class NotificationRecipientTests
{
    [Fact]
    public void NotificationRecipient_Should_Assign_NotificationRecipientId()
    {
        var recipient = new NotificationRecipient();
        var id = Guid.NewGuid();
        recipient.NotificationRecipientId = id;
        recipient.NotificationRecipientId.Should().Be(id);
    }

    [Fact]
    public void NotificationRecipient_Should_Assign_NotificationId()
    {
        var recipient = new NotificationRecipient();
        var notificationId = Guid.NewGuid();
        recipient.NotificationId = notificationId;
        recipient.NotificationId.Should().Be(notificationId);
    }

    [Fact]
    public void NotificationRecipient_Should_Assign_UserId()
    {
        var recipient = new NotificationRecipient();
        var userId = Guid.NewGuid();
        recipient.UserId = userId;
        recipient.UserId.Should().Be(userId);
    }

    [Fact]
    public void NotificationRecipient_Should_Assign_RecipientName()
    {
        var recipient = new NotificationRecipient();
        recipient.RecipientName = "John Doe";
        recipient.RecipientName.Should().Be("John Doe");
    }

    [Fact]
    public void NotificationRecipient_Should_Assign_RecipientEmail()
    {
        var recipient = new NotificationRecipient();
        recipient.RecipientEmail = "john@example.com";
        recipient.RecipientEmail.Should().Be("john@example.com");
    }

    [Fact]
    public void NotificationRecipient_Should_Assign_RecipientMobile()
    {
        var recipient = new NotificationRecipient();
        recipient.RecipientMobile = "9876543210";
        recipient.RecipientMobile.Should().Be("9876543210");
    }

    [Fact]
    public void NotificationRecipient_Should_Assign_DeviceToken()
    {
        var recipient = new NotificationRecipient();
        recipient.DeviceToken = "device-token-123";
        recipient.DeviceToken.Should().Be("device-token-123");
    }

    [Fact]
    public void NotificationRecipient_Should_Assign_RecipientType()
    {
        var recipient = new NotificationRecipient();
        recipient.RecipientType = "Customer";
        recipient.RecipientType.Should().Be("Customer");
    }

    [Fact]
    public void NotificationRecipient_Should_Assign_IsRead()
    {
        var recipient = new NotificationRecipient();
        recipient.IsRead = true;
        recipient.IsRead.Should().BeTrue();
    }

    [Fact]
    public void NotificationRecipient_Should_Assign_ReadDate()
    {
        var recipient = new NotificationRecipient();
        var readDate = DateTimeOffset.UtcNow;
        recipient.ReadDate = readDate;
        recipient.ReadDate.Should().Be(readDate);
    }

    [Fact]
    public void NotificationRecipient_Should_Assign_All_Properties()
    {
        var recipientId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var readDate = DateTimeOffset.UtcNow;
        var recipient = new NotificationRecipient
        {
            NotificationRecipientId = recipientId,
            NotificationId = notificationId,
            UserId = userId,
            RecipientName = "John Doe",
            RecipientEmail = "john@example.com",
            RecipientMobile = "9876543210",
            DeviceToken = "device-token-123",
            RecipientType = "Customer",
            IsRead = true,
            ReadDate = readDate
        };

        recipient.NotificationRecipientId.Should().Be(recipientId);
        recipient.NotificationId.Should().Be(notificationId);
        recipient.UserId.Should().Be(userId);
        recipient.RecipientName.Should().Be("John Doe");
        recipient.RecipientEmail.Should().Be("john@example.com");
        recipient.RecipientMobile.Should().Be("9876543210");
        recipient.DeviceToken.Should().Be("device-token-123");
        recipient.RecipientType.Should().Be("Customer");
        recipient.IsRead.Should().BeTrue();
        recipient.ReadDate.Should().Be(readDate);
    }

    [Fact]
    public void NotificationRecipient_Should_Have_Default_Values()
    {
        var recipient = new NotificationRecipient();
        recipient.NotificationRecipientId.Should().Be(Guid.Empty);
        recipient.NotificationId.Should().Be(Guid.Empty);
        recipient.UserId.Should().Be(Guid.Empty);
        recipient.RecipientName.Should().BeEmpty();
        recipient.RecipientEmail.Should().BeEmpty();
        recipient.RecipientMobile.Should().BeEmpty();
        recipient.DeviceToken.Should().BeEmpty();
        recipient.RecipientType.Should().BeEmpty();
        recipient.IsRead.Should().BeFalse();
        recipient.ReadDate.Should().Be(default);
    }
}