using FluentAssertions;

using BookMyHall.Domain.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class UserSessionTests
{
    [Fact]
    public void UserSession_Should_Assign_UserSessionId()
    {
        var userSession = new UserSession();
        var id = Guid.NewGuid();
        userSession.UserSessionId = id;
        userSession.UserSessionId.Should().Be(id);
    }

    [Fact]
    public void UserSession_Should_Assign_UserId()
    {
        var userSession = new UserSession();
        var userId = Guid.NewGuid();
        userSession.UserId = userId;
        userSession.UserId.Should().Be(userId);
    }

    [Fact]
    public void UserSession_Should_Assign_RefreshTokenId()
    {
        var userSession = new UserSession();
        var refreshTokenId = Guid.NewGuid();
        userSession.RefreshTokenId = refreshTokenId;
        userSession.RefreshTokenId.Should().Be(refreshTokenId);
    }

    [Fact]
    public void UserSession_Should_Assign_DeviceId()
    {
        var userSession = new UserSession();
        var deviceId = Guid.NewGuid();
        userSession.DeviceId = deviceId;
        userSession.DeviceId.Should().Be(deviceId);
    }

    [Fact]
    public void UserSession_Should_Assign_SessionStart()
    {
        var userSession = new UserSession();
        var sessionStart = DateTimeOffset.Now.AddHours(9);
        userSession.SessionStart = sessionStart;
        userSession.SessionStart.Should().Be(sessionStart);
    }

    [Fact]
    public void UserSession_Should_Assign_SessionEnd()
    {
        var userSession = new UserSession();
        var sessionEnd = DateTimeOffset.Now.AddHours(18);
        userSession.SessionEnd = sessionEnd;
        userSession.SessionEnd.Should().Be(sessionEnd);
    }

    [Fact]
    public void UserSession_Should_Assign_LastActivity()
    {
        // Arrange
        var userSession = new UserSession();
        var lastActivity = DateTimeOffset.Now.AddHours(12);

        // Act
        userSession.LastActivity = lastActivity;

        // Assert
        userSession.LastActivity.Should().Be(lastActivity);
    }

    [Fact]
    public void UserSession_Should_Assign_IsActive()
    {
        var userSession = new UserSession();
        userSession.IsActive = true;
        userSession.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UserSession_Should_Assign_All_Properties()
    {
        var userSessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var refreshTokenId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var sessionStart = DateTimeOffset.Now.AddHours(9);
        var sessionEnd = DateTimeOffset.Now.AddHours(18);
        var lastActivity = DateTimeOffset.Now.AddHours(12);

        var userSession = new UserSession
        {
            UserSessionId = userSessionId,
            UserId = userId,
            RefreshTokenId = refreshTokenId,
            DeviceId = deviceId,
            SessionStart = sessionStart,
            SessionEnd = sessionEnd,
            LastActivity = lastActivity,
            IsActive = true
        };

        userSession.UserSessionId.Should().Be(userSessionId);
        userSession.UserId.Should().Be(userId);
        userSession.RefreshTokenId.Should().Be(refreshTokenId);
        userSession.DeviceId.Should().Be(deviceId);
        userSession.SessionStart.Should().Be(sessionStart);
        userSession.SessionEnd.Should().Be(sessionEnd);
        userSession.LastActivity.Should().Be(lastActivity);
        userSession.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UserSession_Should_Have_Default_Values()
    {
        var userSession = new UserSession();
        userSession.UserSessionId.Should().Be(Guid.Empty);
        userSession.UserId.Should().Be(Guid.Empty);
        userSession.RefreshTokenId.Should().Be(Guid.Empty);
        userSession.DeviceId.Should().BeNull();
        userSession.SessionStart.Should().Be(default(DateTimeOffset));
        userSession.LastActivity.Should().Be(default(DateTimeOffset));
        userSession.SessionEnd.Should().BeNull();
        userSession.IsActive.Should().BeTrue();
        userSession.LogoutReason.Should().BeNull();
    }
}