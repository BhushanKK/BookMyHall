using FluentAssertions;
using BookMyHall.Domain.Notifications;

namespace BookMyHall.Domain.Tests.Notifications;

public sealed class NotificationProviderTests
{
    [Fact]
    public void NotificationProvider_Should_Be_Inactive_By_Default()
    {
        var provider = new NotificationProvider();
        provider.IsActive.Should().BeFalse();
    }

    [Fact]
    public void NotificationProvider_Should_Assign_NotificationProviderId()
    {
        var provider = new NotificationProvider();
        var id = Guid.NewGuid();
        provider.NotificationProviderId = id;
        provider.NotificationProviderId.Should().Be(id);
    }

    [Fact]
    public void NotificationProvider_Should_Assign_ProviderCode()
    {
        var provider = new NotificationProvider();
        provider.ProviderCode = "SMTP";
        provider.ProviderCode.Should().Be("SMTP");
    }

    [Fact]
    public void NotificationProvider_Should_Assign_ProviderName()
    {
        var provider = new NotificationProvider();
        provider.ProviderName = "SMTP Provider";
        provider.ProviderName.Should().Be("SMTP Provider");
    }

    [Fact]
    public void NotificationProvider_Should_Assign_Priority()
    {
        var provider = new NotificationProvider();
        provider.Priority = 1;
        provider.Priority.Should().Be(1);
    }

    [Fact]
    public void NotificationProvider_Should_Assign_IsActive()
    {
        var provider = new NotificationProvider();
        provider.IsActive = true;
        provider.IsActive.Should().BeTrue();
    }

    [Fact]
    public void NotificationProvider_Should_Assign_All_Properties()
    {
        var providerId = Guid.NewGuid();
        var provider = new NotificationProvider
        {
            NotificationProviderId = providerId,
            ProviderCode = "SMTP",
            ProviderName = "SMTP Provider",
            Priority = 1,
            IsActive = true
        };

        provider.NotificationProviderId.Should().Be(providerId);
        provider.ProviderCode.Should().Be("SMTP");
        provider.ProviderName.Should().Be("SMTP Provider");
        provider.Priority.Should().Be(1);
        provider.IsActive.Should().BeTrue();
    }

    [Fact]
    public void NotificationProvider_Should_Have_Default_Values()
    {
        var provider = new NotificationProvider();
        provider.NotificationProviderId.Should().Be(Guid.Empty);
        provider.ProviderCode.Should().BeEmpty();
        provider.ProviderName.Should().BeEmpty();
        provider.Priority.Should().Be(0);
        provider.IsActive.Should().BeFalse();
    }
}