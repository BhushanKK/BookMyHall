using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Entities.Identity;

public sealed class EmailVerificationToken : BaseEntity
{
    public Guid EmailVerificationTokenId { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>
    /// Indicates when the email was successfully verified.
    /// Null means the token has not been used.
    /// </summary>
    public DateTimeOffset? VerifiedAt { get; private set; }
    public User User { get; private set; } = default!;
    private EmailVerificationToken()
    {
    }

    public static EmailVerificationToken Create(Guid userId, string tokenHash, DateTimeOffset expiresAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        return new EmailVerificationToken
        {
            EmailVerificationTokenId = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Returns true if the token has expired.
    /// </summary>
    public bool IsExpired() => DateTimeOffset.UtcNow >= ExpiresAt;

    /// <summary>
    /// Returns true if the token has already been used to verify the email.
    /// </summary>
    public bool IsVerified() => VerifiedAt.HasValue;

    /// <summary>
    /// Marks the token as successfully verified.
    /// </summary>
    public void MarkAsVerified()
    {
        if (VerifiedAt is not null)
            throw new InvalidOperationException("The email verification token has already been used.");

        if (IsExpired())
            throw new InvalidOperationException("The email verification token has expired.");

        VerifiedAt = DateTimeOffset.UtcNow;
    }
}