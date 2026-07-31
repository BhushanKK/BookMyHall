using Microsoft.EntityFrameworkCore;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Persistence.Repositories;

public sealed class RefreshTokenRepository(BookMyHallDbContext context)
    : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        => await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

    public Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Update(refreshToken);
        return Task.CompletedTask;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => await context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
        => await context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked && x.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);
}