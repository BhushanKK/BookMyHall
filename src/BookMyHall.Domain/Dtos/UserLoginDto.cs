namespace BookMyHall.Domain.Dtos;

public sealed class UserLoginDto
{
    public Guid UserId { get; init; }
    public string? MobileNumber { get; init; }
    public string? EmailAddress { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public int TokenVersion { get; init; }
    public string? ProfileImageUrl {get;set;}
    public List<JwtRole> Roles { get; init; } = [];
    public bool IsEmailVerified { get; set; }
}