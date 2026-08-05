using BookMyHall.Domain.Audit;

namespace BookMyHall.Infrastructure.Authentication;

public interface IUserLoginHistoryRepository
{
    Task AddAsync(UserLoginHistory entity, CancellationToken cancellationToken);
    Task<UserLoginHistory?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);
    void Update(UserLoginHistory loginHistory);
}