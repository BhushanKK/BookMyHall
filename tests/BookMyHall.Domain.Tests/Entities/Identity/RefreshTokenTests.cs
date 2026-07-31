using FluentAssertions;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class RefreshTokenTests
{
    [Fact]
    public void RefreshToken_Should_Assign_RefreshTokenId()
    {
        var refreshToken = new RefreshToken();
        var id = Guid.NewGuid();

        refreshToken.RefreshTokenId = id;

        refreshToken.RefreshTokenId.Should().Be(id);
    }

    [Fact]
    public void RefreshToken_Should_Assign_UserId()
    {
        var refreshToken = new RefreshToken();
        var userId = Guid.NewGuid();

        refreshToken.UserId = userId;

        refreshToken.UserId.Should().Be(userId);
    }

    [Fact]
    public void RefreshToken_Should_Assign_Token()
    {
        var refreshToken = new RefreshToken();

        refreshToken.Token = "sample-refresh-token";

        refreshToken.Token.Should().Be("sample-refresh-token");
    }

    [Fact]
    public void RefreshToken_Should_Assign_ExpiresAt()
    {
        var refreshToken = new RefreshToken();
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);

        refreshToken.ExpiresAt = expiresAt;

        refreshToken.ExpiresAt.Should().Be(expiresAt);
    }

    [Fact]
    public void RefreshToken_Should_Assign_RevokedAt()
    {
        var refreshToken = new RefreshToken();
        var revokedAt = DateTimeOffset.UtcNow;

        refreshToken.RevokedAt = revokedAt;

        refreshToken.RevokedAt.Should().Be(revokedAt);
    }

    [Fact]
    public void RefreshToken_Should_Assign_All_Properties()
    {
        var refreshTokenId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);
        var revokedAt = DateTimeOffset.UtcNow;

        var refreshToken = new RefreshToken
        {
            RefreshTokenId = refreshTokenId,
            UserId = userId,
            Token = "sample-refresh-token",
            ExpiresAt = expiresAt,
            RevokedAt = revokedAt
        };

        refreshToken.RefreshTokenId.Should().Be(refreshTokenId);
        refreshToken.UserId.Should().Be(userId);
        refreshToken.Token.Should().Be("sample-refresh-token");
        refreshToken.ExpiresAt.Should().Be(expiresAt);
        refreshToken.RevokedAt.Should().Be(revokedAt);
    }

    [Fact]
    public void RefreshToken_Should_Have_Default_Values()
    {
        var refreshToken = new RefreshToken();

        refreshToken.RefreshTokenId.Should().Be(Guid.Empty);
        refreshToken.UserId.Should().Be(Guid.Empty);
        refreshToken.Token.Should().BeEmpty();
        refreshToken.ExpiresAt.Should().Be(default);
        refreshToken.RevokedAt.Should().Be(default);
    }
}