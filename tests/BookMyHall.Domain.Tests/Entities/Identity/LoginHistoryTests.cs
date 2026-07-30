using FluentAssertions;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class LoginHistoryTests
{
    [Fact]
    public void LoginHistory_Should_Assign_LoginHistoryId()
    {
        var loginHistory = new LoginHistory();
        var id = Guid.NewGuid();

        loginHistory.LoginHistoryId = id;

        loginHistory.LoginHistoryId.Should().Be(id);
    }

    [Fact]
    public void LoginHistory_Should_Assign_UserId()
    {
        var loginHistory = new LoginHistory();
        var userId = Guid.NewGuid();

        loginHistory.UserId = userId;

        loginHistory.UserId.Should().Be(userId);
    }

    [Fact]
    public void LoginHistory_Should_Assign_IPAddress()
    {
        var loginHistory = new LoginHistory();

        loginHistory.IPAddress = "192.168.1.1";

        loginHistory.IPAddress.Should().Be("192.168.1.1");
    }

    [Fact]
    public void LoginHistory_Should_Assign_OpratingSystem()
    {
        var loginHistory = new LoginHistory();

        loginHistory.OpratingSystem = "Windows";

        loginHistory.OpratingSystem.Should().Be("Windows");
    }

    [Fact]
    public void LoginHistory_Should_Assign_DeviceType()
    {
        var loginHistory = new LoginHistory();

        loginHistory.DeviceType = "Desktop";

        loginHistory.DeviceType.Should().Be("Desktop");
    }

    [Fact]
    public void LoginHistory_Should_Assign_OperatingSystem()
    {
        var loginHistory = new LoginHistory();

        loginHistory.OperatingSystem = "Windows 11";

        loginHistory.OperatingSystem.Should().Be("Windows 11");
    }

    [Fact]
    public void LoginHistory_Should_Assign_AppVersion()
    {
        var loginHistory = new LoginHistory();

        loginHistory.AppVersion = "1.0.0";

        loginHistory.AppVersion.Should().Be("1.0.0");
    }

    [Fact]
    public void LoginHistory_Should_Assign_LoginTime()
    {
        var loginHistory = new LoginHistory();
        var loginTime = DateTimeOffset.UtcNow;

        loginHistory.LoginTime = loginTime;

        loginHistory.LoginTime.Should().Be(loginTime);
    }

    [Fact]
    public void LoginHistory_Should_Assign_Browser()
    {
        var loginHistory = new LoginHistory();

        loginHistory.Browser = "Chrome";

        loginHistory.Browser.Should().Be("Chrome");
    }

    [Fact]
    public void LoginHistory_Should_Assign_IsSuccessful()
    {
        var loginHistory = new LoginHistory();

        loginHistory.IsSuccessful = true;

        loginHistory.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void LoginHistory_Should_Assign_FailureReason()
    {
        var loginHistory = new LoginHistory();

        loginHistory.FailureReason = "Invalid Password";

        loginHistory.FailureReason.Should().Be("Invalid Password");
    }

    [Fact]
    public void LoginHistory_Should_Assign_All_Properties()
    {
        var loginHistoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var loginTime = DateTimeOffset.UtcNow;

        var loginHistory = new LoginHistory
        {
            LoginHistoryId = loginHistoryId,
            UserId = userId,
            IPAddress = "192.168.1.1",
            OpratingSystem = "Windows",
            DeviceType = "Desktop",
            OperatingSystem = "Windows 11",
            AppVersion = "1.0.0",
            LoginTime = loginTime,
            Browser = "Chrome",
            IsSuccessful = true,
            FailureReason = string.Empty
        };

        loginHistory.LoginHistoryId.Should().Be(loginHistoryId);
        loginHistory.UserId.Should().Be(userId);
        loginHistory.IPAddress.Should().Be("192.168.1.1");
        loginHistory.OpratingSystem.Should().Be("Windows");
        loginHistory.DeviceType.Should().Be("Desktop");
        loginHistory.OperatingSystem.Should().Be("Windows 11");
        loginHistory.AppVersion.Should().Be("1.0.0");
        loginHistory.LoginTime.Should().Be(loginTime);
        loginHistory.Browser.Should().Be("Chrome");
        loginHistory.IsSuccessful.Should().BeTrue();
        loginHistory.FailureReason.Should().BeEmpty();
    }

    [Fact]
    public void LoginHistory_Should_Have_Default_Values()
    {
        var loginHistory = new LoginHistory();

        loginHistory.LoginHistoryId.Should().Be(Guid.Empty);
        loginHistory.UserId.Should().Be(Guid.Empty);
        loginHistory.IPAddress.Should().BeEmpty();
        loginHistory.OpratingSystem.Should().BeEmpty();
        loginHistory.DeviceType.Should().BeEmpty();
        loginHistory.OperatingSystem.Should().BeEmpty();
        loginHistory.AppVersion.Should().BeEmpty();
        loginHistory.LoginTime.Should().Be(default);
        loginHistory.Browser.Should().BeEmpty();
        loginHistory.IsSuccessful.Should().BeFalse();
        loginHistory.FailureReason.Should().BeEmpty();
    }
}