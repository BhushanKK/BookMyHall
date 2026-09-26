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
            .FirstOrDefaultAsync(x => x.ServiceId == serviceId &&!x.IsDeleted &&x.IsActive,cancellationToken);
    public async Task<Service?> GetByNameIncludingDeletedAsync(string serviceName, CancellationToken cancellationToken)
    {
        var normalizedName = serviceName.Trim();
        return await context.Services
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.ServiceName == normalizedName, cancellationToken);
    }
    public async Task<Service?> GetByServiceNameAsync(string serviceName,CancellationToken cancellationToken = default)
        => await context.Services.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ServiceName == serviceName && !x.IsDeleted,cancellationToken);

    public async Task<PaginatedResult<Service>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.Services.AsNoTracking().Where(x=>!x.IsDeleted && x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern= $"%{search}%";
            query = query.Where(x =>EF.Functions.ILike(x.ServiceName,pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.ServiceName)
            .ThenBy(x=>x.ServiceId)
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
     public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, 
        int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Services.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.ServiceName, pattern));
        }

        return await query
            .OrderBy(x => x.ServiceName)
            .ThenBy(x => x.ServiceId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.ServiceId, x.ServiceName))
            .ToListAsync(cancellationToken);
    }
}