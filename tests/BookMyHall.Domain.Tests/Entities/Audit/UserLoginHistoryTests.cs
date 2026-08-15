using FluentAssertions;
using BookMyHall.Domain.Audit;

namespace BookMyHall.Domain.Tests.Entities.Audit;

public sealed class UserLoginHistoryTests
{
    [Fact]
    public void UserLoginHistory_Should_Assign_UserLoginHistoryId()
    {
        var userLoginHistory = new UserLoginHistory();
        var id = Guid.NewGuid();

        userLoginHistory.UserLoginHistoryId = id;

        userLoginHistory.UserLoginHistoryId.Should().Be(id);
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_SessionDurationSeconds()
    {
        var userLoginHistory = new UserLoginHistory();

        userLoginHistory.SessionDurationSeconds = 3600;

        userLoginHistory.SessionDurationSeconds.Should().Be(3600);
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_UserId()
    {
        var userLoginHistory = new UserLoginHistory();
        var userId = Guid.NewGuid();

        userLoginHistory.UserId = userId;

        userLoginHistory.UserId.Should().Be(userId);
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_IpAddress()
    {
        var userLoginHistory = new UserLoginHistory();

        userLoginHistory.IpAddress = "192.168.1.10";

        userLoginHistory.IpAddress.Should().Be("192.168.1.10");
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_LoginDate()
    {
        var userLoginHistory = new UserLoginHistory();
        var loginDate = DateTimeOffset.UtcNow;

        userLoginHistory.LoginDate = loginDate;

        userLoginHistory.LoginDate.Should().Be(loginDate);
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_LogoutDate()
    {
        var userLoginHistory = new UserLoginHistory();
        var logoutDate = DateTimeOffset.UtcNow.AddHours(1);

        userLoginHistory.LogoutDate = logoutDate;

        userLoginHistory.LogoutDate.Should().Be(logoutDate);
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_LoginStatus()
    {
        var userLoginHistory = new UserLoginHistory();

        userLoginHistory.LoginStatus = "Success";

        userLoginHistory.LoginStatus.Should().Be("Success");
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_LoginMethod()
    {
        var userLoginHistory = new UserLoginHistory();

        userLoginHistory.LoginMethod = "Password";

        userLoginHistory.LoginMethod.Should().Be("Password");
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_UserAgent()
    {
        var userLoginHistory = new UserLoginHistory();

        userLoginHistory.UserAgent = "Mozilla/5.0";

        userLoginHistory.UserAgent.Should().Be("Mozilla/5.0");
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_DeviceeType()
    {
        var userLoginHistory = new UserLoginHistory();

        userLoginHistory.DeviceType = "Desktop";

        userLoginHistory.DeviceType.Should().Be("Desktop");
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_OpratingSystem()
    {
        var userLoginHistory = new UserLoginHistory();

        userLoginHistory.OperatingSystem = "Windows 11";

        userLoginHistory.OperatingSystem.Should().Be("Windows 11");
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_Browser()
    {
        var userLoginHistory = new UserLoginHistory();

        userLoginHistory.Browser = "Chrome";

        userLoginHistory.Browser.Should().Be("Chrome");
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_SeesionId()
    {
        var userLoginHistory = new UserLoginHistory();
        var sessionId = Guid.NewGuid();

        userLoginHistory.SessionId = sessionId;

        userLoginHistory.SessionId.Should().Be(sessionId);
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_FailureReason()
    {
        var userLoginHistory = new UserLoginHistory();

        userLoginHistory.FailureReason = "Invalid Password";

        userLoginHistory.FailureReason.Should().Be("Invalid Password");
    }

    [Fact]
    public void UserLoginHistory_Should_Assign_All_Properties()
    {
        var userLoginHistoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var loginDate = DateTimeOffset.UtcNow;
        var logoutDate = loginDate.AddHours(1);

        var userLoginHistory = new UserLoginHistory
        {
            UserLoginHistoryId = userLoginHistoryId,
            UserId = userId,
            IpAddress = "192.168.1.10",
            LoginDate = loginDate,
            LogoutDate = logoutDate,
            LoginStatus = "Success",
            LoginMethod = "Password",
            UserAgent = "Mozilla/5.0",
            DeviceType = "Desktop",
            OperatingSystem = "Windows 11",
            Browser = "Chrome",
            SessionId = sessionId,
            FailureReason = string.Empty
        };

        userLoginHistory.UserLoginHistoryId.Should().Be(userLoginHistoryId);
        userLoginHistory.UserId.Should().Be(userId);
        userLoginHistory.IpAddress.Should().Be("192.168.1.10");
        userLoginHistory.LoginDate.Should().Be(loginDate);
        userLoginHistory.LogoutDate.Should().Be(logoutDate);
        userLoginHistory.LoginStatus.Should().Be("Success");
        userLoginHistory.LoginMethod.Should().Be("Password");
        userLoginHistory.UserAgent.Should().Be("Mozilla/5.0");
        userLoginHistory.DeviceType.Should().Be("Desktop");
        userLoginHistory.OperatingSystem.Should().Be("Windows 11");
        userLoginHistory.Browser.Should().Be("Chrome");
        userLoginHistory.SessionId.Should().Be(sessionId);
        userLoginHistory.FailureReason.Should().BeEmpty();
    }

    [Fact]
    public void UserLoginHistory_Should_Have_Default_Values()
    {
        var userLoginHistory = new UserLoginHistory();

        userLoginHistory.UserLoginHistoryId.Should().Be(Guid.Empty);
        userLoginHistory.UserId.Should().Be(Guid.Empty);
        userLoginHistory.IpAddress.Should().BeEmpty();
        userLoginHistory.LoginDate.Should().Be(default);
        userLoginHistory.LogoutDate.Should().BeNull();
        userLoginHistory.LoginStatus.Should().BeEmpty();
        userLoginHistory.LoginMethod.Should().BeEmpty();
        userLoginHistory.UserAgent.Should().BeEmpty();
        userLoginHistory.DeviceType.Should().BeEmpty();
        userLoginHistory.OperatingSystem.Should().BeEmpty();
        userLoginHistory.Browser.Should().BeEmpty();
        userLoginHistory.SessionId.Should().BeNull();
        userLoginHistory.FailureReason.Should().BeEmpty();
    }
}