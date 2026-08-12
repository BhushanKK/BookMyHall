using BookMyHall.Infrastructure.Options;

namespace BookMyHall.Infrastructure.Tests.Options;

public sealed class EmailOptionsTests
{
    [Fact]
    public void SectionName_ShouldBeEmail()
    {
        // Assert
        Assert.Equal("Email", EmailOptions.SectionName);
    }

    [Fact]
    public void DefaultValues_ShouldBeInitializedCorrectly()
    {
        // Arrange
        var options = new EmailOptions();

        // Assert
        Assert.Equal(string.Empty, options.FromEmail);
        Assert.Equal(string.Empty, options.FromName);
        Assert.Equal(string.Empty, options.Host);
        Assert.Equal(0, options.Port);
        Assert.Equal(string.Empty, options.UserName);
        Assert.Equal(string.Empty, options.Password);
        Assert.False(options.EnableSsl);
        Assert.Equal(string.Empty, options.TemplateFolder);
    }

    [Fact]
    public void Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var options = new EmailOptions
        {
            FromEmail = "noreply@bookmyhall.com",
            FromName = "BookMyHall",
            Host = "smtp.gmail.com",
            Port = 587,
            UserName = "smtp-user",
            Password = "smtp-password",
            EnableSsl = true,
            TemplateFolder = "EmailTemplates"
        };

        // Assert
        Assert.Equal("noreply@bookmyhall.com", options.FromEmail);
        Assert.Equal("BookMyHall", options.FromName);
        Assert.Equal("smtp.gmail.com", options.Host);
        Assert.Equal(587, options.Port);
        Assert.Equal("smtp-user", options.UserName);
        Assert.Equal("smtp-password", options.Password);
        Assert.True(options.EnableSsl);
        Assert.Equal("EmailTemplates", options.TemplateFolder);
    }

    [Fact]
    public void EnableSsl_ShouldSupportTrueAndFalse()
    {
        // Arrange
        var options = new EmailOptions();

        // Act
        options.EnableSsl = true;

        // Assert
        Assert.True(options.EnableSsl);

        // Act
        options.EnableSsl = false;

        // Assert
        Assert.False(options.EnableSsl);
    }

    [Fact]
    public void Port_ShouldSupportValidSmtpPort()
    {
        // Arrange
        var options = new EmailOptions();

        // Act
        options.Port = 587;

        // Assert
        Assert.Equal(587, options.Port);
    }

    [Fact]
    public void AllProperties_ShouldBeMutable()
    {
        // Arrange
        var options = new EmailOptions();

        // Act
        options.FromEmail = "rakesh@BookMyHall.com";
        options.FromName = "Test";
        options.Host = "smtp.BookMyHall.com";
        options.Port = 465;
        options.UserName = "username";
        options.Password = "password";
        options.EnableSsl = true;
        options.TemplateFolder = "Templates";

        // Assert
        Assert.Equal("rakesh@BookMyHall.com", options.FromEmail);
        Assert.Equal("Test", options.FromName);
        Assert.Equal("smtp.BookMyHall.com", options.Host);
        Assert.Equal(465, options.Port);
        Assert.Equal("username", options.UserName);
        Assert.Equal("password", options.Password);
        Assert.True(options.EnableSsl);
        Assert.Equal("Templates", options.TemplateFolder);
    }
}