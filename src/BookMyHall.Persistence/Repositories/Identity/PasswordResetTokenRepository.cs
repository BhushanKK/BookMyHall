using Microsoft.EntityFrameworkCore;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Identity;

namespace BookMyHall.Persistence.Repositories.Identity;

public sealed class PasswordResetTokenRepository(BookMyHallDbContext dbContext) : IPasswordResetTokenRepository
{
    public async Task AddAsync(PasswordResetToken passwordResetToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(passwordResetToken);
        await dbContext.PasswordResetTokens.AddAsync(passwordResetToken, cancellationToken);
    }

    public async Task<PasswordResetToken?> GetActiveTokenAsync(Guid userId, string tokenHash, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);
        return await dbContext.PasswordResetTokens
            .FirstOrDefaultAsync(
                x => x.UserId == userId
                  && x.TokenHash == tokenHash
                  && x.UsedAt == null
                  && x.ExpiresAt > DateTimeOffset.UtcNow,
                cancellationToken);
    }

    public async Task<IEnumerable<PasswordResetToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.PasswordResetTokens
            .Where(x =>
                x.UserId == userId &&
                x.UsedAt == null &&
                x.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public Task DeleteAsync(PasswordResetToken passwordResetToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(passwordResetToken);
        dbContext.PasswordResetTokens.Remove(passwordResetToken);
        return Task.CompletedTask;
    }
    public async Task DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await dbContext.PasswordResetTokens
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        if (tokens.Count > 0)
            dbContext.PasswordResetTokens.RemoveRange(tokens);
    }

    public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await dbContext.PasswordResetTokens
            .Where(x => x.ExpiresAt <= DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);

        if (expiredTokens.Count > 0)
            dbContext.PasswordResetTokens.RemoveRange(expiredTokens);
    }
}

