using BookMyHall.Domain.Entities.Identity;

public sealed class RefreshToken
{
    public Guid RefreshTokenId { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public Guid? RevokedBy { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public User User { get; set; } = default!;
}