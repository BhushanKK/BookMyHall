namespace BookMyHall.Domain.Dtos;
public sealed class JwtRole
{
    public Guid RoleId { get; init; }

    public string RoleName { get; init; } = string.Empty;
}