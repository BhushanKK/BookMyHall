using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IUserSessionRepository
{
    Task AddAsync(UserSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByRefreshTokenIdAsync(Guid refreshTokenId, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByIdAsync(Guid userSessionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserSession>> GetActiveSessionsAsync(Guid userId,CancellationToken cancellationToken = default);
    Task RevokeAllSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task EndAllSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
}