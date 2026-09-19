using BookMyHall.Application.Abstractions.Audit;
using BookMyHall.Domain.Audit;
using BookMyHall.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Infrastructure.Audit;
public sealed class ApiRequestLogService(IDbContextFactory<BookMyHallDbContext> dbContextFactory): IApiRequestLogService
{
    public async Task LogAsync(ApiRequestLog log,CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(log);
        await using var dbContext =await dbContextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.ApiRequestLogs.AddAsync(log,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}