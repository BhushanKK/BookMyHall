using System.Net;
using BookMyHall.Infrastructure.Authentication;
using Microsoft.AspNetCore.Http;

namespace BookMyHall.Infrastructure.Tests.Authentication;

public sealed class ClientInfoServiceTests
{
    private static ClientInfoService CreateService(string? ipAddress = null,string? userAgent = null)
    {
        var httpContext = new DefaultHttpContext();
        if (ipAddress is not null)
        {
            httpContext.Connection.RemoteIpAddress =IPAddress.Parse(ipAddress);
        }

        if (userAgent is not null)
        {
            httpContext.Request.Headers.UserAgent = userAgent;
        }

        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        return new ClientInfoService(accessor);
    }

    #region IpAddress

    [Fact]
    public void IpAddress_ShouldReturnRemoteIpAddress()
    {
        // Arrange
        var service = CreateService("192.168.1.100");

        // Act
        var result = service.IpAddress;

        // Assert
        Assert.Equal("192.168.1.100", result);
    }

    [Fact]
    public void IpAddress_ShouldReturnNull_WhenHttpContextIsNull()
    {
        // Arrange
        var accessor = new HttpContextAccessor
        {
            HttpContext = null
        };

        var service = new ClientInfoService(accessor);

        // Act
        var result = service.IpAddress;

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region UserAgent

    [Fact]
    public void UserAgent_ShouldReturnRequestUserAgent()
    {
        // Arrange
        const string userAgent ="Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0";

        var service = CreateService(userAgent: userAgent);

        // Act
        var result = service.UserAgent;

        // Assert
        Assert.Equal(userAgent, result);
    }

    [Fact]
    public void UserAgent_ShouldReturnNull_WhenHttpContextIsNull()
    {
        // Arrange
        var accessor = new HttpContextAccessor
        {
            HttpContext = null
        };

        var service = new ClientInfoService(accessor);

        // Act
        var result = service.UserAgent;

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Browser

    [Theory]
    [InlineData("Mozilla/5.0 Edg/120.0", "Microsoft Edge")]
    [InlineData("Mozilla/5.0 Chrome/120.0", "Chrome")]
    [InlineData("Mozilla/5.0 Firefox/120.0", "Firefox")]
    [InlineData("Mozilla/5.0 Safari/537.36", "Safari")]
    [InlineData("Mozilla/5.0 OPR/100.0", "Opera")]
    [InlineData("Mozilla/5.0 Opera/100.0", "Opera")]
    [InlineData("SomeUnknownBrowser/1.0", "Unknown")]
    public void Browser_ShouldDetectBrowser(string userAgent,string expected)
    {
        // Arrange
        var service = CreateService(userAgent: userAgent);

        // Act
        var result = service.Browser;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Browser_ShouldReturnNull_WhenUserAgentIsMissing()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.Browser;

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region OperatingSystem

    [Theory]
    [InlineData("Mozilla/5.0 Windows NT 10.0", "Windows")]
    [InlineData("Mozilla/5.0 Android 13", "Android")]
    [InlineData("Mozilla/5.0 iPhone OS 17_0", "iOS")]
    [InlineData("Mozilla/5.0 iPad OS 17_0", "iOS")]
    [InlineData("Mozilla/5.0 Macintosh; Intel Mac OS X", "macOS")]
    [InlineData("Mozilla/5.0 Mac OS X", "macOS")]
    [InlineData("Mozilla/5.0 X11; Linux x86_64", "Linux")]
    [InlineData("SomeUnknownOS/1.0", "Unknown")]
    public void OperatingSystem_ShouldDetectOperatingSystem(string userAgent,string expected)
    {
        // Arrange
        var service = CreateService(userAgent: userAgent);

        // Act
        var result = service.OperatingSystem;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void OperatingSystem_ShouldReturnNull_WhenUserAgentIsMissing()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.OperatingSystem;

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region DeviceType

    [Theory]
    [InlineData("Mozilla/5.0 iPad", "Tablet")]
    [InlineData("Mozilla/5.0 Tablet", "Tablet")]
    [InlineData("Mozilla/5.0 Android Mobile", "Mobile")]
    [InlineData("Mozilla/5.0 iPhone", "Mobile")]
    [InlineData("Mozilla/5.0 Mobile", "Mobile")]
    [InlineData("Mozilla/5.0 Windows NT 10.0 Win64 x64", "Desktop")]
    [InlineData("SomeUnknownDevice/1.0", "Desktop")]
    public void DeviceType_ShouldDetectDeviceType(string userAgent,string expected)
    {
        // Arrange
        var service = CreateService(userAgent: userAgent);

        // Act
        var result = service.DeviceType;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DeviceType_ShouldReturnNull_WhenUserAgentIsMissing()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.DeviceType;

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region LoginSource

    [Theory]
    [InlineData("Mozilla/5.0 Android 13","Android")]
    [InlineData("Mozilla/5.0 iPhone OS 17_0","iOS")]
    [InlineData("Mozilla/5.0 iPad OS 17_0","iOS")]
    [InlineData("Mozilla/5.0 Windows NT 10.0 Chrome/120.0","Web")]
    [InlineData("Mozilla/5.0 Macintosh; Intel Mac OS X","Web")]
    public void LoginSource_ShouldDetectLoginSource(string userAgent,string expected)
    {
        // Arrange
        var service = CreateService(userAgent: userAgent);

        // Act
        var result = service.LoginSource;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void LoginSource_ShouldReturnApi_WhenUserAgentIsMissing()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.LoginSource;

        // Assert
        Assert.Equal("API", result);
    }

    #endregion

    #region Combined Client Information

    [Fact]
    public void ClientInfo_ShouldReturnExpectedInformation_ForChromeWindowsDesktop()
    {
        // Arrange
        const string userAgent ="Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0";

        var service = CreateService(ipAddress: "10.0.0.25",userAgent: userAgent);

        // Act & Assert
        Assert.Equal("10.0.0.25", service.IpAddress);
        Assert.Equal(userAgent, service.UserAgent);
        Assert.Equal("Chrome", service.Browser);
        Assert.Equal("Windows", service.OperatingSystem);
        Assert.Equal("Desktop", service.DeviceType);
        Assert.Equal("Web", service.LoginSource);
    }

    [Fact]
    public void ClientInfo_ShouldReturnExpectedInformation_ForAndroid()
    {
        // Arrange
        const string userAgent ="Mozilla/5.0 (Linux; Android 13; Mobile) Chrome/120.0";

        var service = CreateService(ipAddress: "192.168.1.50",userAgent: userAgent);

        // Act & Assert
        Assert.Equal("192.168.1.50", service.IpAddress);
        Assert.Equal(userAgent, service.UserAgent);
        Assert.Equal("Chrome", service.Browser);
        Assert.Equal("Android", service.OperatingSystem);
        Assert.Equal("Mobile", service.DeviceType);
        Assert.Equal("Android", service.LoginSource);
    }

    [Fact]
    public void ClientInfo_ShouldReturnExpectedInformation_ForIPhone()
    {
        // Arrange
        const string userAgent ="Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) Safari/605.1";

        var service = CreateService(ipAddress: "172.16.0.10",userAgent: userAgent);

        // Act & Assert
        Assert.Equal("172.16.0.10", service.IpAddress);
        Assert.Equal(userAgent, service.UserAgent);
        Assert.Equal("Safari", service.Browser);
        Assert.Equal("iOS", service.OperatingSystem);
        Assert.Equal("Mobile", service.DeviceType);
        Assert.Equal("iOS", service.LoginSource);
    }

    #endregion
}