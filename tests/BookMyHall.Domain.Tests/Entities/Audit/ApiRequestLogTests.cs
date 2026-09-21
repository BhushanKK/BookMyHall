
using FluentAssertions;
using BookMyHall.Domain.Audit;

namespace BookMyHall.Domain.Tests.Entities.Audit;

public sealed class ApiRequestLogTests
{
    [Fact]
    public void ApiRequestLog_Should_Have_Default_Values()
    {
        // Arrange
        var apiRequestLog = new ApiRequestLog();

        // Assert
        apiRequestLog.ApiRequestLogId
            .Should()
            .Be(Guid.Empty);

        apiRequestLog.UserId
            .Should()
            .BeNull();

        apiRequestLog.CorrelationId
            .Should()
            .Be(Guid.Empty);

        apiRequestLog.HttpMethod
            .Should()
            .BeEmpty();

        apiRequestLog.RequestPath
            .Should()
            .BeEmpty();

        apiRequestLog.QueryString
            .Should()
            .BeEmpty();

        apiRequestLog.RequestIpAddress
            .Should()
            .BeEmpty();

        apiRequestLog.UserAgent
            .Should()
            .BeEmpty();

        apiRequestLog.StatusCode
            .Should()
            .Be(0);

        apiRequestLog.ExecutionTimeMs
            .Should()
            .Be(0);

        apiRequestLog.IsSuccess
            .Should()
            .BeFalse();

        apiRequestLog.ErrorMessage
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_All_Properties()
    {
        // Arrange
        var apiRequestLogId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var apiRequestLog = new ApiRequestLog
        {
            ApiRequestLogId = apiRequestLogId,
            UserId = userId,
            CorrelationId = correlationId,
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

        // Assert
        apiRequestLog.ApiRequestLogId
            .Should()
            .Be(apiRequestLogId);

        apiRequestLog.UserId
            .Should()
            .Be(userId);

        apiRequestLog.CorrelationId
            .Should()
            .Be(correlationId);

        apiRequestLog.HttpMethod
            .Should()
            .Be("POST");

        apiRequestLog.RequestPath
            .Should()
            .Be("/api/auth/login");

        apiRequestLog.QueryString
            .Should()
            .Be("?returnUrl=/dashboard");

        apiRequestLog.RequestIpAddress
            .Should()
            .Be("192.168.1.10");

        apiRequestLog.UserAgent
            .Should()
            .Be("Mozilla/5.0");

        apiRequestLog.StatusCode
            .Should()
            .Be(200);

        apiRequestLog.ExecutionTimeMs
            .Should()
            .Be(150);

        apiRequestLog.IsSuccess
            .Should()
            .BeTrue();

        apiRequestLog.ErrorMessage
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void ApiRequestLog_Should_Assign_Inherited_Audit_Properties()
    {
        // Arrange
        var createdBy = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();
        var createdDate = DateTimeOffset.UtcNow;
        var updatedDate = createdDate.AddMinutes(5);

        var apiRequestLog = new ApiRequestLog
        {
            CreatedBy = createdBy,
            CreatedDate = createdDate,
            UpdatedBy = updatedBy,
            UpdatedDate = updatedDate
        };

        // Assert
        apiRequestLog.CreatedBy
            .Should()
            .Be(createdBy);

        apiRequestLog.CreatedDate
            .Should()
            .Be(createdDate);

        apiRequestLog.UpdatedBy
            .Should()
            .Be(updatedBy);

        apiRequestLog.UpdatedDate
            .Should()
            .Be(updatedDate);
    }

    [Fact]
    public void ApiRequestLog_Should_Allow_Null_Updated_Audit_Properties()
    {
        // Arrange
        var apiRequestLog = new ApiRequestLog
        {
            CreatedBy = Guid.NewGuid(),
            CreatedDate = DateTimeOffset.UtcNow,
            UpdatedBy = null,
            UpdatedDate = null
        };

        // Assert
        apiRequestLog.UpdatedBy
            .Should()
            .BeNull();

        apiRequestLog.UpdatedDate
            .Should()
            .BeNull();
    }

    [Fact]
    public void ApiRequestLog_Should_Allow_Unsuccessful_Request()
    {
        // Arrange
        var apiRequestLog = new ApiRequestLog
        {
            StatusCode = 500,
            ExecutionTimeMs = 250,
            IsSuccess = false,
            ErrorMessage = "Internal Server Error"
        };

        // Assert
        apiRequestLog.StatusCode
            .Should()
            .Be(500);

        apiRequestLog.ExecutionTimeMs
            .Should()
            .Be(250);

        apiRequestLog.IsSuccess
            .Should()
            .BeFalse();

        apiRequestLog.ErrorMessage
            .Should()
            .Be("Internal Server Error");
    }
}

