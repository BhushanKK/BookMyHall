using Microsoft.AspNetCore.Http;
using BookMyHall.Application.Abstractions.Authentication;

namespace BookMyHall.Infrastructure.Authentication;

public sealed class ClientInfoService(IHttpContextAccessor httpContextAccessor)
    : IClientInfoService
{
    private HttpContext? HttpContext => httpContextAccessor.HttpContext;

    public string? IpAddress
    {
        get
        {
            return HttpContext?
                .Connection
                .RemoteIpAddress?
                .ToString();
        }
    }

    public string? UserAgent
    {
        get
        {
            return HttpContext?
                .Request
                .Headers
                .UserAgent
                .ToString();
        }
    }

    public string? Browser
    {
        get
        {
            var userAgent = UserAgent;

            if (string.IsNullOrWhiteSpace(userAgent))
                return null;

            if (userAgent.Contains("Edg/", StringComparison.OrdinalIgnoreCase))
                return "Microsoft Edge";

            if (userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                return "Chrome";

            if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase))
                return "Firefox";

            if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase)
                && !userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                return "Safari";

            if (userAgent.Contains("Opera", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("OPR", StringComparison.OrdinalIgnoreCase))
                return "Opera";

            return "Unknown";
        }
    }

    public string? OperatingSystem
    {
        get
        {
            var userAgent = UserAgent;

            if (string.IsNullOrWhiteSpace(userAgent))
                return null;

            if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                return "Windows";

            if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
                return "Android";

            if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                return "iOS";

            if (userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("Mac OS", StringComparison.OrdinalIgnoreCase))
                return "macOS";

            if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
                return "Linux";

            return "Unknown";
        }
    }

    public string? DeviceType
    {
        get
        {
            var userAgent = UserAgent;

            if (string.IsNullOrWhiteSpace(userAgent))
                return null;

            if (userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
            {
                return "Tablet";
            }

            if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
            {
                return "Mobile";
            }

            return "Desktop";
        }
    }

    public string LoginSource
    {
        get
        {
            var userAgent = UserAgent;

            if (string.IsNullOrWhiteSpace(userAgent))
                return "API";

            if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
                return "Android";

            if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase)
                || userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                return "iOS";

            return "Web";
        }
    }
}