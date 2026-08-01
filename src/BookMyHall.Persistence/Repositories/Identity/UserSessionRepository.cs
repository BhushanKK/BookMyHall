using Microsoft.EntityFrameworkCore;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories.Identity;

public sealed class UserSessionRepository(BookMyHallDbContext context)
    : IUserSessionRepository
{
    public async Task AddAsync(UserSession session, CancellationToken cancellationToken = default)
        => await context.UserSessions.AddAsync(session, cancellationToken);
    public Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        context.UserSessions.Update(session);
        return Task.CompletedTask;
    }

    public async Task<UserSession?> GetByRefreshTokenIdAsync(Guid refreshTokenId, CancellationToken cancellationToken = default)
    {
        return await context.UserSessions
            .FirstOrDefaultAsync(x => x.RefreshTokenId == refreshTokenId, cancellationToken);
    }

    public async Task<UserSession?> GetByIdAsync(Guid userSessionId, CancellationToken cancellationToken = default)
    {
        return await context.UserSessions
            .FirstOrDefaultAsync(x => x.UserSessionId == userSessionId, cancellationToken);
    }

    public async Task<IReadOnlyList<UserSession>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.UserSessions
            .Where(x =>
                x.UserId == userId &&
                x.IsActive)
            .OrderByDescending(x => x.LastActivity)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllSessionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var sessions = await context.UserSessions
            .Where(x =>
                x.UserId == userId &&
                x.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.IsActive = false;
            session.SessionEnd = DateTimeOffset.UtcNow;
        }
    }

    public async Task EndAllSessionsAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        var sessions = await context.UserSessions
            .Where(x => x.UserId == userId && x.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.IsActive = false;
            session.SessionEnd = DateTimeOffset.UtcNow;
            session.LastActivity = DateTimeOffset.UtcNow;
        }
    }
}