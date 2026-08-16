namespace BookMyHall.Application.Features.Identity.Authentication;
public sealed class RefreshTokenWithUserDto
{
    public Guid RefreshTokenId { get; set; }
    public string Token { get; init; } = null!;
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public Guid RevokedBy {get;set;}
    public DateTimeOffset RevokedAt {get;set;}
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string MobileNumber { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public int TokenVersion { get; set; }
    public bool IsActive { get; init; }
    public List<string> Roles { get; init; } = [];
}