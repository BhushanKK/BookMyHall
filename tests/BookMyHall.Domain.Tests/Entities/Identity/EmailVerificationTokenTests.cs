using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class EmailVerificationTokenTests
{
    [Fact]
    public void Create_ShouldCreateToken_WithExpectedValues()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tokenHash = "hashed-token";
        var expiresAt = DateTimeOffset.UtcNow.AddHours(1);

        // Act
        var token = EmailVerificationToken.Create(
            userId,
            tokenHash,
            expiresAt);

        // Assert
        Assert.NotEqual(Guid.Empty, token.EmailVerificationTokenId);
        Assert.Equal(userId, token.UserId);
        Assert.Equal(tokenHash, token.TokenHash);
        Assert.Equal(expiresAt, token.ExpiresAt);
        Assert.Null(token.VerifiedAt);
    }

    [Fact]
    public void Create_ShouldThrow_WhenTokenHashIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddHours(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            EmailVerificationToken.Create(userId,null!,expiresAt));
    }

    [Fact]
    public void Create_ShouldThrow_WhenTokenHashIsEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddHours(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            EmailVerificationToken.Create(userId,string.Empty,expiresAt));
    }

    [Fact]
    public void Create_ShouldThrow_WhenTokenHashIsWhiteSpace()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddHours(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            EmailVerificationToken.Create(userId,"   ",expiresAt));
    }

    [Fact]
    public void IsExpired_ShouldReturnFalse_WhenTokenHasNotExpired()
    {
        // Arrange
        var token = EmailVerificationToken.Create(
            Guid.NewGuid(),
            "hashed-token",
            DateTimeOffset.UtcNow.AddMinutes(10));

        // Act
        var result = token.IsExpired();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsExpired_ShouldReturnTrue_WhenExpirationTimeIsInPast()
    {
        // Arrange
        var token = EmailVerificationToken.Create(Guid.NewGuid(),"hashed-token",
            DateTimeOffset.UtcNow.AddMinutes(-10));

        // Act
        var result = token.IsExpired();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsExpired_ShouldReturnTrue_WhenExpirationTimeIsNow()
    {
        // Arrange
        var token = EmailVerificationToken.Create(
            Guid.NewGuid(),
            "hashed-token",
            DateTimeOffset.UtcNow);

        // Act
        var result = token.IsExpired();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsVerified_ShouldReturnFalse_WhenTokenHasNotBeenVerified()
    {
        // Arrange
        var token = EmailVerificationToken.Create(Guid.NewGuid(),"hashed-token",DateTimeOffset.UtcNow.AddHours(1));

        // Act
        var result = token.IsVerified();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void MarkAsVerified_ShouldSetVerifiedAt()
    {
        // Arrange
        var token = EmailVerificationToken.Create(Guid.NewGuid(),"hashed-token",DateTimeOffset.UtcNow.AddHours(1));

        // Act
        token.MarkAsVerified();

        // Assert
        Assert.True(token.IsVerified());
        Assert.NotNull(token.VerifiedAt);
    }

    [Fact]
    public void MarkAsVerified_ShouldSetVerifiedAtToCurrentTime()
    {
        // Arrange
        var before = DateTimeOffset.UtcNow;
        var token = EmailVerificationToken.Create(
            Guid.NewGuid(),
            "hashed-token",
            DateTimeOffset.UtcNow.AddHours(1));

        // Act
        token.MarkAsVerified();

        var after = DateTimeOffset.UtcNow;

        // Assert
        Assert.NotNull(token.VerifiedAt);
        Assert.InRange(token.VerifiedAt.Value,before,after);
    }

    [Fact]
    public void MarkAsVerified_ShouldThrow_WhenTokenIsAlreadyVerified()
    {
        // Arrange
        var token = EmailVerificationToken.Create(Guid.NewGuid(),"hashed-token",DateTimeOffset.UtcNow.AddHours(1));
        token.MarkAsVerified();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => token.MarkAsVerified());
        Assert.Equal("The email verification token has already been used.",exception.Message);
    }

    [Fact]
    public void MarkAsVerified_ShouldThrow_WhenTokenHasExpired()
    {
        // Arrange
        var token = EmailVerificationToken.Create(Guid.NewGuid(),"hashed-token",DateTimeOffset.UtcNow.AddMinutes(-1));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => token.MarkAsVerified());

        Assert.Equal("The email verification token has expired.",exception.Message);
    }

    [Fact]
    public void MarkAsVerified_ShouldNotMarkTokenAsVerified_WhenTokenHasExpired()
    {
        // Arrange
        var token = EmailVerificationToken.Create(Guid.NewGuid(),"hashed-token",DateTimeOffset.UtcNow.AddMinutes(-1));

        // Act
        Assert.Throws<InvalidOperationException>(() => token.MarkAsVerified());

        // Assert
        Assert.False(token.IsVerified());
        Assert.Null(token.VerifiedAt);
    }

    [Fact]
    public void MarkAsVerified_ShouldNotChangeVerifiedAt_WhenCalledTwice()
    {
        // Arrange
        var token = EmailVerificationToken.Create(Guid.NewGuid(),"hashed-token",DateTimeOffset.UtcNow.AddHours(1));
        token.MarkAsVerified();
        var firstVerifiedAt = token.VerifiedAt;

        // Act
        Assert.Throws<InvalidOperationException>(() => token.MarkAsVerified());

        // Assert
        Assert.Equal(firstVerifiedAt, token.VerifiedAt);
    }
}