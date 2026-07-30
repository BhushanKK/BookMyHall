using FluentAssertions;
using BookMyHall.Domain.Audit;

namespace BookMyHall.Domain.Tests.Entities.Audit;

public sealed class ErrorLogTests
{
    [Fact]
    public void ErrorLog_Should_Assign_ErrorLogId()
    {
        var errorLog = new ErrorLog();
        var id = Guid.NewGuid();

        errorLog.ErrorLogId = id;

        errorLog.ErrorLogId.Should().Be(id);
    }

    [Fact]
    public void ErrorLog_Should_Assign_CorrelationId()
    {
        var errorLog = new ErrorLog();
        var correlationId = Guid.NewGuid();

        errorLog.CorrelationId = correlationId;

        errorLog.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void ErrorLog_Should_Assign_UserId()
    {
        var errorLog = new ErrorLog();
        var userId = Guid.NewGuid();

        errorLog.UserId = userId;

        errorLog.UserId.Should().Be(userId);
    }

    [Fact]
    public void ErrorLog_Should_Assign_RequestPath()
    {
        var errorLog = new ErrorLog();

        errorLog.RequestPath = "/api/users";

        errorLog.RequestPath.Should().Be("/api/users");
    }

    [Fact]
    public void ErrorLog_Should_Assign_HttpMethod()
    {
        var errorLog = new ErrorLog();

        errorLog.HttpMethod = "GET";

        errorLog.HttpMethod.Should().Be("GET");
    }

    [Fact]
    public void ErrorLog_Should_Assign_ExceptionType()
    {
        var errorLog = new ErrorLog();

        errorLog.ExceptionType = "System.Exception";

        errorLog.ExceptionType.Should().Be("System.Exception");
    }

    [Fact]
    public void ErrorLog_Should_Assign_ErrorMessage()
    {
        var errorLog = new ErrorLog();

        errorLog.ErrorMessage = "An unexpected error occurred.";

        errorLog.ErrorMessage.Should().Be("An unexpected error occurred.");
    }

    [Fact]
    public void ErrorLog_Should_Assign_StackTrace()
    {
        var errorLog = new ErrorLog();

        errorLog.StackTrace = "Stack trace details";

        errorLog.StackTrace.Should().Be("Stack trace details");
    }

    [Fact]
    public void ErrorLog_Should_Assign_InnerException()
    {
        var errorLog = new ErrorLog();

        errorLog.InnerException = "Inner exception message";

        errorLog.InnerException.Should().Be("Inner exception message");
    }

    [Fact]
    public void ErrorLog_Should_Assign_IpAddress()
    {
        var errorLog = new ErrorLog();

        errorLog.IpAddress = "192.168.1.10";

        errorLog.IpAddress.Should().Be("192.168.1.10");
    }

    [Fact]
    public void ErrorLog_Should_Assign_UserAgent()
    {
        var errorLog = new ErrorLog();

        errorLog.UserAgent = "Mozilla/5.0";

        errorLog.UserAgent.Should().Be("Mozilla/5.0");
    }

    [Fact]
    public void ErrorLog_Should_Assign_All_Properties()
    {
        var errorLogId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var errorLog = new ErrorLog
        {
            ErrorLogId = errorLogId,
            CorrelationId = correlationId,
            UserId = userId,
            RequestPath = "/api/users",
            HttpMethod = "POST",
            ExceptionType = "System.InvalidOperationException",
            ErrorMessage = "Operation failed.",
            StackTrace = "Stack trace details",
            InnerException = "Inner exception details",
            IpAddress = "192.168.1.10",
            UserAgent = "Mozilla/5.0"
        };

        errorLog.ErrorLogId.Should().Be(errorLogId);
        errorLog.CorrelationId.Should().Be(correlationId);
        errorLog.UserId.Should().Be(userId);
        errorLog.RequestPath.Should().Be("/api/users");
        errorLog.HttpMethod.Should().Be("POST");
        errorLog.ExceptionType.Should().Be("System.InvalidOperationException");
        errorLog.ErrorMessage.Should().Be("Operation failed.");
        errorLog.StackTrace.Should().Be("Stack trace details");
        errorLog.InnerException.Should().Be("Inner exception details");
        errorLog.IpAddress.Should().Be("192.168.1.10");
        errorLog.UserAgent.Should().Be("Mozilla/5.0");
    }

    [Fact]
    public void ErrorLog_Should_Have_Default_Values()
    {
        var errorLog = new ErrorLog();

        errorLog.ErrorLogId.Should().Be(Guid.Empty);
        errorLog.CorrelationId.Should().Be(Guid.Empty);
        errorLog.UserId.Should().Be(Guid.Empty);
        errorLog.RequestPath.Should().BeEmpty();
        errorLog.HttpMethod.Should().BeNull();
        errorLog.ExceptionType.Should().BeNull();
        errorLog.ErrorMessage.Should().BeNull();
        errorLog.StackTrace.Should().BeEmpty();
        errorLog.InnerException.Should().BeNull();
        errorLog.IpAddress.Should().BeNull();
        errorLog.UserAgent.Should().BeNull();
    }
}