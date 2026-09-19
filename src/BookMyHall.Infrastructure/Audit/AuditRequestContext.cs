using BookMyHall.Application.Abstractions.Audit;
using Microsoft.AspNetCore.Http;

namespace BookMyHall.Infrastructure.Audit;

public sealed class AuditRequestContext(IHttpContextAccessor httpContextAccessor): IAuditRequestContext
{
    private const string CorrelationIdItem = "CorrelationId";

    private HttpContext? HttpContext =>
        httpContextAccessor.HttpContext;

    public Guid CorrelationId
    {
        get
        {
            if (HttpContext?.Items[CorrelationIdItem]
                is Guid correlationId)
            {
                return correlationId;
            }

            return Guid.Empty;
        }
    }

    public string? IpAddress =>
        HttpContext?.Connection.RemoteIpAddress?.ToString();

    public string? UserAgent =>
        HttpContext?.Request.Headers.UserAgent.FirstOrDefault();
}