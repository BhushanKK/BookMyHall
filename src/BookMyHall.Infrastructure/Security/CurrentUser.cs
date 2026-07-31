using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BookMyHall.Application.Abstractions.Security;

namespace BookMyHall.Infrastructure.Security;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public string? FullName => User?.FindFirstValue(ClaimTypes.Name);

    public string? MobileNumber => User?.FindFirstValue(ClaimTypes.MobilePhone);

    public string? EmailAddress => User?.FindFirstValue(ClaimTypes.Email);

    public IReadOnlyList<string> Roles =>
        User?
            .FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList()
        ?? [];
}