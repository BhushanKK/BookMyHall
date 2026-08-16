using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class PasswordResetTokenTests
{
    [Fact]
    public void Create_ShouldCreateToken_WithValidData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tokenHash = "hashed-token";
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(30);
        var ipAddress = "192.168.1.10";
        var userAgent = "Mozilla/5.0";

        // Act
        var token = PasswordResetToken.Create(
            userId,
            tokenHash,
            expiresAt,
            ipAddress,
            userAgent);

        // Assert
        Assert.NotNull(token);
        Assert.NotEqual(Guid.Empty, token.PasswordResetTokenId);
        Assert.Equal(userId, token.UserId);
        Assert.Equal(tokenHash, token.TokenHash);
        Assert.Equal(expiresAt, token.ExpiresAt);
        Assert.Null(token.UsedAt);
        Assert.Equal(ipAddress, token.CreatedByIpAddress);
        Assert.Equal(userAgent, token.CreatedByUserAgent);
        Assert.True(token.CreatedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_ShouldAllowNullIpAddressAndUserAgent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(30);

        // Act
        var token = PasswordResetToken.Create(userId,"hashed-token",expiresAt);

        // Assert
        Assert.Null(token.CreatedByIpAddress);
        Assert.Null(token.CreatedByUserAgent);
    }

    [Fact]
    public void Create_ShouldThrow_WhenTokenHashIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(30);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            PasswordResetToken.Create(userId, null!,expiresAt));
    }

    [Fact]
    public void Create_ShouldThrow_WhenTokenHashIsEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(30);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            PasswordResetToken.Create(userId,string.Empty,expiresAt));
    }

    [Fact]
    public void Create_ShouldThrow_WhenTokenHashIsWhitespace()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(30);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            PasswordResetToken.Create(userId,"   ",expiresAt));
    }

    [Fact]
    public void Create_ShouldThrow_WhenExpirationIsInThePast()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(-1);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            PasswordResetToken.Create(userId,"hashed-token",expiresAt));

        Assert.Contains("Expiration time must be in the future.",exception.Message);
    }

    [Fact]
    public void Create_ShouldThrow_WhenExpirationIsNow()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            PasswordResetToken.Create(userId,"hashed-token",expiresAt));
    }

    [Fact]
    public void IsExpired_ShouldReturnFalse_WhenTokenHasNotExpired()
    {
        // Arrange
        var token = PasswordResetToken.Create(
            Guid.NewGuid(),
            "hashed-token",
            DateTimeOffset.UtcNow.AddMinutes(30));

        // Act
        var result = token.IsExpired();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsExpired_ShouldReturnTrue_WhenTokenHasExpired()
    {
        // Arrange
        var token = PasswordResetToken.Create(
            Guid.NewGuid(),
            "hashed-token",
            DateTimeOffset.UtcNow.AddSeconds(1));

        // Allow the token to expire
        Thread.Sleep(TimeSpan.FromSeconds(2));

        // Act
        var result = token.IsExpired();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsActive_ShouldReturnTrue_WhenTokenIsUnusedAndNotExpired()
    {
        // Arrange
        var token = PasswordResetToken.Create(
            Guid.NewGuid(),
            "hashed-token",
            DateTimeOffset.UtcNow.AddMinutes(30));

        // Act
        var result = token.IsActive();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsActive_ShouldReturnFalse_WhenTokenIsExpired()
    {
        // Arrange
        var token = PasswordResetToken.Create(
            Guid.NewGuid(),
            "hashed-token",
            DateTimeOffset.UtcNow.AddMilliseconds(1));

        Thread.Sleep(10);

        // Act
        var result = token.IsActive();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void MarkAsUsed_ShouldSetUsedAt()
    {
        // Arrange
        var token = PasswordResetToken.Create(
            Guid.NewGuid(),
            "hashed-token",
            DateTimeOffset.UtcNow.AddMinutes(30));

        // Act
        token.MarkAsUsed();

        // Assert
        Assert.NotNull(token.UsedAt);
        Assert.True(token.UsedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void MarkAsUsed_ShouldMakeTokenInactive()
    {
        // Arrange
        var token = PasswordResetToken.Create(Guid.NewGuid(),"hashed-token",DateTimeOffset.UtcNow.AddMinutes(30));

        // Act
        token.MarkAsUsed();

        // Assert
        Assert.False(token.IsActive());
    }

    [Fact]
    public void MarkAsUsed_ShouldThrow_WhenTokenAlreadyUsed()
    {
        // Arrange
        var token = PasswordResetToken.Create(Guid.NewGuid(),"hashed-token",DateTimeOffset.UtcNow.AddMinutes(30));
        token.MarkAsUsed();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>token.MarkAsUsed());

        Assert.Equal("Password reset token has already been used.",exception.Message);
    }

    [Fact]
    public void IsActive_ShouldRemainFalse_AfterTokenIsUsed()
    {
        // Arrange
        var token = PasswordResetToken.Create(Guid.NewGuid(),"hashed-token",
            DateTimeOffset.UtcNow.AddMinutes(30));

        // Act
        token.MarkAsUsed();

        // Assert
        Assert.False(token.IsActive());
        Assert.False(token.IsExpired());
        Assert.NotNull(token.UsedAt);
    }
}