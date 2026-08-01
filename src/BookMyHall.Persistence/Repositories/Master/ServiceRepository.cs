using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class ServiceRepository(BookMyHallDbContext context): IServiceRepository
{
    public async Task AddAsync(Service service,CancellationToken cancellationToken = default)
        => await context.Services.AddAsync( service,cancellationToken);

    public Task UpdateAsync(Service service,CancellationToken cancellationToken = default)
    {
        context.Services.Update(service);
        return Task.CompletedTask;
    }

    public async Task<Service?> GetByIdAsync(Guid serviceId,CancellationToken cancellationToken = default)
        => await context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ServiceId == serviceId,
                cancellationToken);

    public async Task<Service?> GetByServiceNameAsync(string serviceName,CancellationToken cancellationToken = default)
        => await context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ServiceName == serviceName,
                cancellationToken);

    public async Task<PaginatedResult<Service>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        IQueryable<Service> query = context.Services
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(
                    x.ServiceName,
                    $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.ServiceName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Service>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}