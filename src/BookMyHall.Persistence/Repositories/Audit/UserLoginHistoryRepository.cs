using BookMyHall.Domain.Audit;
using BookMyHall.Infrastructure.Authentication;
using BookMyHall.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories.Audit;

public sealed class UserLoginHistoryRepository(BookMyHallDbContext dbContext) : IUserLoginHistoryRepository
{
    public async Task AddAsync(UserLoginHistory entity, CancellationToken cancellationToken)
        => await dbContext.UserLoginHistories.AddAsync(entity, cancellationToken);
    public async Task<UserLoginHistory?> GetBySessionIdAsync(Guid sessionId,CancellationToken cancellationToken)
        => await dbContext.UserLoginHistories.FirstOrDefaultAsync(x => x.SessionId == sessionId,cancellationToken);
    public void Update(UserLoginHistory entity)
        => dbContext.UserLoginHistories.Update(entity);
}