using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories;

public sealed class VendorServiceAreaRepository(BookMyHallDbContext context) : IVendorServiceAreaRepository
{
    public async Task AddAsync(VendorServiceArea vendorServiceArea, CancellationToken cancellationToken = default)
    {
        await context.VendorServiceAreas.AddAsync(vendorServiceArea, cancellationToken);
    }

    public Task UpdateAsync(VendorServiceArea vendorServiceArea, CancellationToken cancellationToken = default)
    {
        context.VendorServiceAreas.Update(vendorServiceArea);
        return Task.CompletedTask;
    }

    public async Task<VendorServiceArea?> GetByIdAsync(Guid vendorServiceAreaId, CancellationToken cancellationToken = default)
    {
        return await context.VendorServiceAreas
            .FirstOrDefaultAsync(x => x.VendorServiceAreaId == vendorServiceAreaId, cancellationToken);
    }
    public async Task<VendorServiceArea?>GetByLocationIncludingDeletedAsync(Guid vendorId, Guid? stateId,Guid? cityId,
               Guid? areaId,CancellationToken cancellationToken = default)
    {
        return await context.VendorServiceAreas
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.VendorId == vendorId && x.StateId == stateId && x.CityId == cityId &&
                    x.AreaId == areaId, cancellationToken);
    }
    public async Task<bool> ExistsAsync(Guid vendorId,Guid? stateId,Guid? cityId,Guid? areaId,
       Guid? excludeVendorServiceAreaId, CancellationToken cancellationToken = default)
    {
        var query = context.VendorServiceAreas
            .Where(x => x.VendorId == vendorId && x.StateId == stateId &&
                    x.CityId == cityId && x.AreaId == areaId);

        if (excludeVendorServiceAreaId.HasValue)
        {
            query = query.Where(x => x.VendorServiceAreaId != excludeVendorServiceAreaId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<PaginatedResult<VendorServiceArea>> GetAllAsync(PaginationRequest request,
        Guid? vendorId, CancellationToken cancellationToken = default)
    {
        var query = context.VendorServiceAreas
            .AsNoTracking()
            .AsQueryable();

        if (vendorId.HasValue)
        {
            query = query.Where(x => x.VendorId == vendorId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "serviceradiuskm" =>
                request.SortDescending
                    ? query.OrderByDescending( x => x.ServiceRadiusKm)
                    : query.OrderBy( x => x.ServiceRadiusKm),

            "isprimary" =>
                request.SortDescending
                    ? query.OrderByDescending( x => x.IsPrimary)
                    : query.OrderBy( x => x.IsPrimary),

            _ =>
                query.OrderBy( x => x.VendorId)
        };

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<VendorServiceArea>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}