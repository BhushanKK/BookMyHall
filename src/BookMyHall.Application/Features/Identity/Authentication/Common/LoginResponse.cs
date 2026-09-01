using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class LoginResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? EmailAddress { get; set; }
    public IReadOnlyCollection<JwtRole> Roles { get; set; } = [];
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public string? ProfileImageUrl {get;set;}
    public bool IsEmailVerified { get; set; }
}