using System.Text.Json;
using BookMyHall.Api.Middleware;
using BookMyHall.Contracts.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace BookMyHall.Api.Tests.Middleware;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_Call_Next_When_No_Exception_Occurs()
    {
        // Arrange
        var nextCalled = false;

        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ExceptionHandlingMiddleware(next);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_Should_Return_InternalServerError_When_Exception_Occurs()
    {
        // Arrange
        RequestDelegate next = _ =>
        {
            throw new InvalidOperationException("Something went wrong.");
        };

        var middleware = new ExceptionHandlingMiddleware(next);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode
            .Should()
            .Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task InvokeAsync_Should_Set_ContentType_To_ApplicationJson_When_Exception_Occurs()
    {
        // Arrange
        RequestDelegate next = _ =>
        {
            throw new Exception("Unexpected error.");
        };

        var middleware = new ExceptionHandlingMiddleware(next);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task InvokeAsync_Should_Return_Expected_Error_Response()
    {
        // Arrange
        RequestDelegate next = _ =>
        {
            throw new Exception("Database connection failed.");
        };

        var middleware = new ExceptionHandlingMiddleware(next);

        var context = new DefaultHttpContext();
        context.TraceIdentifier = "test-trace-id";
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Position = 0;

        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();

        var response = JsonSerializer.Deserialize<ApiErrorResponse>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        response.Message.Should().Be("An unexpected error occurred.");
        response.TraceId.Should().Be("test-trace-id");
    }

    [Fact]
    public async Task InvokeAsync_Should_Not_Expose_Exception_Message()
    {
        // Arrange
        const string sensitiveExceptionMessage =
            "Database password or connection information.";

        RequestDelegate next = _ =>
        {
            throw new Exception(sensitiveExceptionMessage);
        };

        var middleware = new ExceptionHandlingMiddleware(next);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Position = 0;

        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        responseBody.Should().NotContain(sensitiveExceptionMessage);
        responseBody.Should().Contain("An unexpected error occurred.");
    }
}