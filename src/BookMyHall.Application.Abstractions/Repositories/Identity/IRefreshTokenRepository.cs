using BookMyHall.Application.Features.Identity.Authentication;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshTokenWithUserDto?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all active refresh tokens for a user.
    /// Used when changing password or logging out from all devices.
    /// </summary>
    Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a specific refresh token.
    /// Used during logout or refresh token rotation.
    /// </summary>
    Task RevokeAsync(Guid refreshTokenId, Guid revokedBy, CancellationToken cancellationToken = default);
}