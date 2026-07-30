using FluentAssertions;
using BookMyHall.Domain.Notifications;

namespace BookMyHall.Domain.Tests.Notifications;

public sealed class NotificationPreferenceTests
{
    [Fact]
    public void NotificationPreference_Should_Assign_NotificationPreferenceId()
    {
        var preference = new NotificationPreference();
        var id = Guid.NewGuid();
        preference.NotificationPreferenceId = id;
        preference.NotificationPreferenceId.Should().Be(id);
    }

    [Fact]
    public void NotificationPreference_Should_Assign_UserId()
    {
        var preference = new NotificationPreference();
        var userId = Guid.NewGuid();
        preference.UserId = userId;
        preference.UserId.Should().Be(userId);
    }

    [Fact]
    public void NotificationPreference_Should_Assign_IsEmailEnabled()
    {
        var preference = new NotificationPreference();
        preference.IsEmailEnabled = true;
        preference.IsEmailEnabled.Should().BeTrue();
    }

    [Fact]
    public void NotificationPreference_Should_Assign_IsSmsEnabled()
    {
        var preference = new NotificationPreference();
        preference.IsSmsEnabled = true;
        preference.IsSmsEnabled.Should().BeTrue();
    }

    [Fact]
    public void NotificationPreference_Should_Assign_IsPushEnabled()
    {
        var preference = new NotificationPreference();
        preference.IsPushEnabled = true;
        preference.IsPushEnabled.Should().BeTrue();
    }

    [Fact]
    public void NotificationPreference_Should_Assign_IsInAppEnabled()
    {
        var preference = new NotificationPreference();
        preference.IsInAppEnabled = true;
        preference.IsInAppEnabled.Should().BeTrue();
    }

    [Fact]
    public void NotificationPreference_Should_Assign_IsWhatsAppEnabled()
    {
        var preference = new NotificationPreference();
        preference.IsWhatsAppEnabled = true;
        preference.IsWhatsAppEnabled.Should().BeTrue();
    }

    [Fact]
    public void NotificationPreference_Should_Assign_All_Properties()
    {
        var preferenceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var preference = new NotificationPreference
        {
            NotificationPreferenceId = preferenceId,
            UserId = userId,
            IsEmailEnabled = true,
            IsSmsEnabled = true,
            IsPushEnabled = true,
            IsInAppEnabled = false,
            IsWhatsAppEnabled = true
        };

        preference.NotificationPreferenceId.Should().Be(preferenceId);
        preference.UserId.Should().Be(userId);
        preference.IsEmailEnabled.Should().BeTrue();
        preference.IsSmsEnabled.Should().BeTrue();
        preference.IsPushEnabled.Should().BeTrue();
        preference.IsInAppEnabled.Should().BeFalse();
        preference.IsWhatsAppEnabled.Should().BeTrue();
    }

    [Fact]
    public void NotificationPreference_Should_Have_Default_Values()
    {
        var preference = new NotificationPreference();
        preference.NotificationPreferenceId.Should().Be(Guid.Empty);
        preference.UserId.Should().Be(Guid.Empty);
        preference.IsEmailEnabled.Should().BeFalse();
        preference.IsSmsEnabled.Should().BeFalse();
        preference.IsPushEnabled.Should().BeFalse();
        preference.IsInAppEnabled.Should().BeFalse();
        preference.IsWhatsAppEnabled.Should().BeFalse();
    }
}