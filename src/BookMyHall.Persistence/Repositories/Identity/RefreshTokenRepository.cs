using Microsoft.EntityFrameworkCore;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Dtos;

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

    public async Task<RefreshTokenWithUserDto?> GetByTokenAsync(
    string token,
    CancellationToken cancellationToken = default)
    {
        return await context.RefreshTokens
            .AsNoTracking()
            .Where(x => x.Token == token)
            .Select(x => new RefreshTokenWithUserDto
            {
                RefreshTokenId = x.RefreshTokenId,
                Token = x.Token,
                ExpiresAt = x.ExpiresAt,
                IsRevoked = x.IsRevoked,

                UserId = x.User.UserId,
                FullName = x.User.FullName,
                MobileNumber = x.User.MobileNumber!,
                EmailAddress = x.User.EmailAddress!,
                TokenVersion = x.User.TokenVersion,
                IsActive = x.User.IsActive,

                Roles = x.User.UserRoles
                .Select(ur => new JwtRole
                {
                    RoleId = ur.Role.RoleId,
                    RoleName = ur.Role.RoleName
                })
                .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
        => await context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked && x.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);

    public async Task RevokeAllByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        var refreshTokens = await context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTimeOffset.UtcNow;
            token.RevokedBy = userId;
        }
    }
    public async Task RevokeAsync(
        Guid refreshTokenId,
        Guid revokedBy,
        CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens
        .Where(x => x.RefreshTokenId == refreshTokenId)
        .ExecuteUpdateAsync(
            setters => setters
                .SetProperty(x => x.IsRevoked, true)
                .SetProperty(x => x.RevokedAt, DateTimeOffset.UtcNow)
                .SetProperty(x => x.RevokedBy, revokedBy),
            cancellationToken);
    }
}