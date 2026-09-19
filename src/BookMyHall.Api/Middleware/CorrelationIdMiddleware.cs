namespace BookMyHall.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-Id";
    private const string ItemName = "CorrelationId";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId =
            Guid.TryParse(
                context.Request.Headers[HeaderName].FirstOrDefault(),
                out var existingCorrelationId)
                    ? existingCorrelationId
                    : Guid.NewGuid();

        context.Items[ItemName] = correlationId;

        context.Response.Headers[HeaderName] =
            correlationId.ToString();

        await next(context);
    }
}