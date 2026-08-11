using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class UserPreferenceTests
{
    [Fact]
    public void Create_ShouldCreateUserPreference_WithExpectedUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var preference = UserPreference.Create(userId);

        // Assert
        Assert.NotEqual(Guid.Empty, preference.UserPreferenceId);
        Assert.Equal(userId, preference.UserId);
    }

    [Fact]
    public void Create_ShouldSetDefaultCurrencyCode()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var preference = UserPreference.Create(userId);

        // Assert
        Assert.Equal("INR", preference.CurrencyCode);
    }

    [Fact]
    public void Create_ShouldSetDefaultTimeZone()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var preference = UserPreference.Create(userId);

        // Assert
        Assert.Equal("Asia/Kolkata", preference.TimeZone);
    }

    [Fact]
    public void Create_ShouldSetDefaultDateFormat()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var preference = UserPreference.Create(userId);

        // Assert
        Assert.Equal("dd-MM-yyyy", preference.DateFormat);
    }

    [Fact]
    public void Create_ShouldSetDefaultTimeFormat()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var preference = UserPreference.Create(userId);

        // Assert
        Assert.Equal("24", preference.TimeFormat);
    }

    [Fact]
    public void Create_ShouldSetDefaultLanguageCode()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var preference = UserPreference.Create(userId);

        // Assert
        Assert.Equal("en-IN", preference.LanguageCode);
    }

    [Fact]
    public void Create_ShouldEnableEmailNotificationByDefault()
    {
        // Arrange
        var preference = UserPreference.Create(Guid.NewGuid());

        // Assert
        Assert.True(preference.EmailNotification);
    }

    [Fact]
    public void Create_ShouldEnableSmsNotificationByDefault()
    {
        // Arrange
        var preference = UserPreference.Create(Guid.NewGuid());

        // Assert
        Assert.True(preference.SmsNotification);
    }

    [Fact]
    public void Create_ShouldEnablePushNotificationByDefault()
    {
        // Arrange
        var preference = UserPreference.Create(Guid.NewGuid());

        // Assert
        Assert.True(preference.PushNotification);
    }

    [Fact]
    public void Create_ShouldSetDefaultTheme()
    {
        // Arrange
        var preference = UserPreference.Create(Guid.NewGuid());

        // Assert
        Assert.Equal("Light", preference.Theme);
    }

    [Fact]
    public void Create_ShouldGenerateDifferentIds_ForDifferentPreferences()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var first = UserPreference.Create(userId);
        var second = UserPreference.Create(userId);

        // Assert
        Assert.NotEqual(first.UserPreferenceId, second.UserPreferenceId);
    }
}