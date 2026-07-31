using System.Net;
using System.Text.Json;
using BookMyHall.Contracts.Common;
using Serilog;

namespace BookMyHall.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            Log.Error(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response =
            new ApiErrorResponse(
                context.Response.StatusCode,
                "An unexpected error occurred.",
                context.TraceIdentifier,
                DateTimeOffset.UtcNow
            );

        var json =JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}