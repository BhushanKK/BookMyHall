using System.Diagnostics;
using BookMyHall.Application.Abstractions.Audit;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Domain.Audit;

namespace BookMyHall.Api.Middleware;

public sealed class AuditLogMiddleware(
    RequestDelegate next,
    IApiRequestLogService apiRequestLogService,
    ICurrentUser currentUser,
    IAuditRequestContext auditRequestContext)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var startTimestamp = Stopwatch.GetTimestamp();

        Exception? exception = null;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            var elapsed =
                Stopwatch.GetElapsedTime(startTimestamp);

            var executionTimeMs =
                elapsed.TotalMilliseconds >= int.MaxValue
                    ? int.MaxValue
                    : (int)Math.Max(
                        0,
                        elapsed.TotalMilliseconds);

            try
            {
                await apiRequestLogService.LogAsync(new ApiRequestLog
                    {
                        ApiRequestLogId = Guid.NewGuid(),

                        UserId =
                            currentUser.UserId
                            ?? Guid.Empty,

                        CorrelationId =
                            auditRequestContext
                                .CorrelationId
                                .ToString(),

                        HttpMethod =
                            context.Request.Method,

                        RequestPath =
                            context.Request.Path.Value
                            ?? string.Empty,

                        QueryString =
                            context.Request.QueryString.Value
                            ?? string.Empty,

                        RequestIpAddress =
                            auditRequestContext.IpAddress
                            ?? string.Empty,

                        UserAgent =
                            auditRequestContext.UserAgent
                            ?? string.Empty,

                        StatusCode =
                            context.Response.StatusCode,

                        ExecutionTimeMs =
                            executionTimeMs,

                        IsSuccess =
                            exception is null &&
                            context.Response.StatusCode < 400,

                        ErrorMessage =
                            exception?.Message
                            ?? string.Empty
                    },
                    CancellationToken.None);
            }
            catch
            {
                // Audit logging must not replace
                // the original request exception.
            }
        }
    }
}