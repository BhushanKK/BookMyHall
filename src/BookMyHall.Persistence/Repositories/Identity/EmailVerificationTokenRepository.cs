using Microsoft.EntityFrameworkCore;

using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Identity;

namespace BookMyHall.Persistence.Repositories.Identity;

public sealed class EmailVerificationTokenRepository(BookMyHallDbContext dbContext)
    : IEmailVerificationTokenRepository
{
    public async Task AddAsync(EmailVerificationToken emailVerificationToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(emailVerificationToken);
        await dbContext.EmailVerificationTokens.AddAsync(emailVerificationToken, cancellationToken);
    }

    public async Task<EmailVerificationToken?> GetActiveTokenAsync(Guid userId, string tokenHash, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        return await dbContext.EmailVerificationTokens
            .FirstOrDefaultAsync(
                x => x.UserId == userId
                  && x.TokenHash == tokenHash
                  && x.VerifiedAt == null
                  && x.ExpiresAt > DateTimeOffset.UtcNow,
                cancellationToken);
    }

    public async Task<IEnumerable<EmailVerificationToken>> GetActiveTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.EmailVerificationTokens
            .Where(x =>
                x.UserId == userId &&
                x.VerifiedAt == null &&
                x.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public Task DeleteAsync(EmailVerificationToken emailVerificationToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(emailVerificationToken);
        dbContext.EmailVerificationTokens.Remove(emailVerificationToken);
        return Task.CompletedTask;
    }

    public async Task DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await dbContext.EmailVerificationTokens
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        if (tokens.Count > 0)
            dbContext.EmailVerificationTokens.RemoveRange(tokens);
    }

    public async Task DeleteExpiredAsync(
        CancellationToken cancellationToken = default)
    {
        var expiredTokens = await dbContext.EmailVerificationTokens
            .Where(x =>
                x.VerifiedAt == null &&
                x.ExpiresAt <= DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);

        if (expiredTokens.Count > 0)
            dbContext.EmailVerificationTokens.RemoveRange(expiredTokens);
    }

    public async Task<EmailVerificationToken?> GetVerifiedTokenAsync(Guid userId, string tokenHash,
        CancellationToken cancellationToken = default)
        => await dbContext.EmailVerificationTokens
        .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.TokenHash == tokenHash &&
                    x.VerifiedAt != null &&
                    x.ExpiresAt > DateTimeOffset.UtcNow,
                cancellationToken);
}