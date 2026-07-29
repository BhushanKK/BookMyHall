using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence;

public sealed class UnitOfWork(BookMyHallDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);
}