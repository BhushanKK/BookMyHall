using BookMyHall.Domain.Entities.Identity;

using FluentAssertions;

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

    [Fact]
    public void Update_Should_Update_All_User_Preferences()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var preference = UserPreference.Create(userId);

        // Act
        preference.Update(
            currencyCode: "USD",
            timeZone: "America/New_York",
            dateFormat: "MM/dd/yyyy",
            timeFormat: "12",
            languageCode: "en-US",
            emailNotification: false,
            smsNotification: false,
            pushNotification: true,
            theme: "Dark");

        // Assert
        preference.CurrencyCode.Should().Be("USD");
        preference.TimeZone.Should().Be("America/New_York");
        preference.DateFormat.Should().Be("MM/dd/yyyy");
        preference.TimeFormat.Should().Be("12");
        preference.LanguageCode.Should().Be("en-US");
        preference.EmailNotification.Should().BeFalse();
        preference.SmsNotification.Should().BeFalse();
        preference.PushNotification.Should().BeTrue();
        preference.Theme.Should().Be("Dark");
    }

    [Fact]
    public void UpdateNotificationPreferences_Should_Update_All_Notification_Settings()
    {
        // Arrange
        var preference = UserPreference.Create(Guid.NewGuid());

        // Act
        preference.UpdateNotificationPreferences(
            emailNotification: false,
            smsNotification: false,
            pushNotification: false);

        // Assert
        preference.EmailNotification.Should().BeFalse();
        preference.SmsNotification.Should().BeFalse();
        preference.PushNotification.Should().BeFalse();
    }

    [Fact]
    public void UpdateNotificationPreferences_Should_Allow_Enabling_All_Notifications()
    {
        // Arrange
        var preference = UserPreference.Create(Guid.NewGuid());

        preference.UpdateNotificationPreferences(
            emailNotification: false,
            smsNotification: false,
            pushNotification: false);

        // Act
        preference.UpdateNotificationPreferences(
            emailNotification: true,
            smsNotification: true,
            pushNotification: true);

        // Assert
        preference.EmailNotification.Should().BeTrue();
        preference.SmsNotification.Should().BeTrue();
        preference.PushNotification.Should().BeTrue();
    }

    [Fact]
    public void UpdateRegionalSettings_Should_Update_CurrencyCode_And_TimeZone()
    {
        // Arrange
        var preference = UserPreference.Create(Guid.NewGuid());

        // Act
        preference.UpdateRegionalSettings(
            currencyCode: "USD",
            timeZone: "America/New_York");

        // Assert
        preference.CurrencyCode.Should().Be("USD");
        preference.TimeZone.Should().Be("America/New_York");
    }

    [Fact]
    public void UpdateLanguage_Should_Update_LanguageCode()
    {
        // Arrange
        var preference = UserPreference.Create(Guid.NewGuid());

        // Act
        preference.UpdateLanguage("fr-FR");

        // Assert
        preference.LanguageCode.Should().Be("fr-FR");
    }

    [Fact]
    public void UpdateDateTimeFormat_Should_Update_DateFormat_And_TimeFormat()
    {
        // Arrange
        var preference = UserPreference.Create(Guid.NewGuid());

        // Act
        preference.UpdateDateTimeFormat(
            dateFormat: "MM/dd/yyyy",
            timeFormat: "12");

        // Assert
        preference.DateFormat.Should().Be("MM/dd/yyyy");
        preference.TimeFormat.Should().Be("12");
    }

    [Fact]
    public void UpdateTheme_Should_Update_Theme()
    {
        // Arrange
        var preference = UserPreference.Create(Guid.NewGuid());

        // Act
        preference.UpdateTheme("Dark");

        // Assert
        preference.Theme.Should().Be("Dark");
    }
}