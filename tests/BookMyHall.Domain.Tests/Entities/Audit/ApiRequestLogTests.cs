using FluentAssertions;
using BookMyHall.Domain.Audit;

namespace BookMyHall.Domain.Tests.Entities.Audit;

public sealed class ApiRequestLogTests
{
    [Fact]
    public void ApiRequestLog_Should_Assign_ApiRequestLogId()
    {
        var apiRequestLog = new ApiRequestLog();
        var id = Guid.NewGuid();

        apiRequestLog.ApiRequestLogId = id;

        apiRequestLog.ApiRequestLogId.Should().Be(id);
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_UserId()
    {
        var apiRequestLog = new ApiRequestLog();
        var userId = Guid.NewGuid();

        apiRequestLog.UserId = userId;

        apiRequestLog.UserId.Should().Be(userId);
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_CorrelationId()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.CorrelationId = "corr-12345";

        apiRequestLog.CorrelationId.Should().Be("corr-12345");
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_HttpMethod()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.HttpMethod = "GET";

        apiRequestLog.HttpMethod.Should().Be("GET");
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_RequestPath()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.RequestPath = "/api/users";

        apiRequestLog.RequestPath.Should().Be("/api/users");
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_QueryString()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.QueryString = "?page=1";

        apiRequestLog.QueryString.Should().Be("?page=1");
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_RequestIpAddress()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.RequestIpAddress = "192.168.1.10";

        apiRequestLog.RequestIpAddress.Should().Be("192.168.1.10");
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_UserAgent()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.UserAgent = "Mozilla/5.0";

        apiRequestLog.UserAgent.Should().Be("Mozilla/5.0");
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_StatusCode()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.StatusCode = 200;

        apiRequestLog.StatusCode.Should().Be(200);
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_ExecutionTimeMs()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.ExecutionTimeMs = 125;

        apiRequestLog.ExecutionTimeMs.Should().Be(125);
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_IsSuccess()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.IsSuccess = true;

        apiRequestLog.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_ErrorMessage()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.ErrorMessage = "Internal Server Error";

        apiRequestLog.ErrorMessage.Should().Be("Internal Server Error");
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_All_Properties()
    {
        var apiRequestLogId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var apiRequestLog = new ApiRequestLog
        {
            ApiRequestLogId = apiRequestLogId,
            UserId = userId,
            CorrelationId = "corr-12345",
            HttpMethod = "POST",
            RequestPath = "/api/auth/login",
            QueryString = "?returnUrl=/dashboard",
            RequestIpAddress = "192.168.1.10",
            UserAgent = "Mozilla/5.0",
            StatusCode = 200,
            ExecutionTimeMs = 150,
            IsSuccess = true,
            ErrorMessage = string.Empty
        };

        apiRequestLog.ApiRequestLogId.Should().Be(apiRequestLogId);
        apiRequestLog.UserId.Should().Be(userId);
        apiRequestLog.CorrelationId.Should().Be("corr-12345");
        apiRequestLog.HttpMethod.Should().Be("POST");
        apiRequestLog.RequestPath.Should().Be("/api/auth/login");
        apiRequestLog.QueryString.Should().Be("?returnUrl=/dashboard");
        apiRequestLog.RequestIpAddress.Should().Be("192.168.1.10");
        apiRequestLog.UserAgent.Should().Be("Mozilla/5.0");
        apiRequestLog.StatusCode.Should().Be(200);
        apiRequestLog.ExecutionTimeMs.Should().Be(150);
        apiRequestLog.IsSuccess.Should().BeTrue();
        apiRequestLog.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public void ApiRequestLog_Should_Have_Default_Values()
    {
        var apiRequestLog = new ApiRequestLog();

        apiRequestLog.ApiRequestLogId.Should().Be(Guid.Empty);
        apiRequestLog.UserId.Should().Be(Guid.Empty);
        apiRequestLog.CorrelationId.Should().BeEmpty();
        apiRequestLog.HttpMethod.Should().BeEmpty();
        apiRequestLog.RequestPath.Should().BeEmpty();
        apiRequestLog.QueryString.Should().BeEmpty();
        apiRequestLog.RequestIpAddress.Should().BeEmpty();
        apiRequestLog.UserAgent.Should().BeEmpty();
        apiRequestLog.StatusCode.Should().Be(0);
        apiRequestLog.ExecutionTimeMs.Should().Be(0);
        apiRequestLog.IsSuccess.Should().BeFalse();
        apiRequestLog.ErrorMessage.Should().BeEmpty();
    }
}