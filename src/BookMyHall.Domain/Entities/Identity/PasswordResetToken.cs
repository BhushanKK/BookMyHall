namespace BookMyHall.Domain.Entities.Identity;

public sealed class PasswordResetToken
{
    private PasswordResetToken()
    {
    }
    public Guid PasswordResetTokenId { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? UsedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string? CreatedByIpAddress { get; private set; }
    public string? CreatedByUserAgent { get; private set; }
    
    #region Navigation Properties
    public User User { get; private set; } = null!;
    #endregion
    #region Factory Methods
    public static PasswordResetToken Create(
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAt,
        string? createdByIpAddress = null,
        string? createdByUserAgent = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);
        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new ArgumentException("Expiration time must be in the future.", nameof(expiresAt));

        return new PasswordResetToken
        {
            PasswordResetTokenId = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedByIpAddress = createdByIpAddress,
            CreatedByUserAgent = createdByUserAgent
        };
    }

    #endregion

    #region Business Methods

    public void MarkAsUsed()
    {
        if (UsedAt.HasValue)
            throw new InvalidOperationException("Password reset token has already been used.");
        UsedAt = DateTimeOffset.UtcNow;
    }

    public bool IsExpired()
        => DateTimeOffset.UtcNow >= ExpiresAt;

    public bool IsActive()
        => UsedAt is null && !IsExpired();
    #endregion
}
