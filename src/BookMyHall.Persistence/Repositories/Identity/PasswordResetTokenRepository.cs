using Microsoft.EntityFrameworkCore;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Identity;

namespace BookMyHall.Persistence.Repositories.Identity;
public sealed class PasswordResetTokenRepository(BookMyHallDbContext dbContext):IPasswordResetTokenRepository
{
    public async Task AddAsync(PasswordResetToken passwordResetToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(passwordResetToken);
        await dbContext.PasswordResetTokens.AddAsync(passwordResetToken, cancellationToken);
    }

    public async Task<PasswordResetToken?> GetActiveTokenAsync(Guid userId, string tokenHash, CancellationToken cancellationToken = default)
    {
        var now= DateTimeOffset.UtcNow;
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);
        return await dbContext.PasswordResetTokens.AsNoTracking()
        .FirstOrDefaultAsync(x=>x.UserId==userId && x.TokenHash == tokenHash && x.UsedAt==null&& x.ExpiresAt>now,cancellationToken);
    }

    public async Task<IReadOnlyList<PasswordResetToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now= DateTimeOffset.UtcNow;
        return await dbContext.PasswordResetTokens
            .Where(x =>x.UserId == userId &&x.UsedAt == null &&x.ExpiresAt > now)
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
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default)
    {
        var now= DateTimeOffset.UtcNow;
        var expiredTokens = await dbContext.PasswordResetTokens
            .Where(x => x.ExpiresAt <= now)
            .ExecuteDeleteAsync(cancellationToken);
    }
}

