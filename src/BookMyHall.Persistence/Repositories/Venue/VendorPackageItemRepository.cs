using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories;

public sealed class VendorPackageItemRepository(BookMyHallDbContext context): IVendorPackageItemRepository
{
    public async Task AddAsync(VendorPackageItem vendorPackageItem,CancellationToken cancellationToken = default)
    {
        await context.VendorPackageItems.AddAsync(vendorPackageItem,cancellationToken);
    }

    public Task UpdateAsync(VendorPackageItem vendorPackageItem,CancellationToken cancellationToken = default)
    {
        context.VendorPackageItems.Update(vendorPackageItem);
        return Task.CompletedTask;
    }

    public async Task<VendorPackageItem?> GetByIdAsync(Guid vendorPackageItemId,CancellationToken cancellationToken = default)
    {
        return await context.VendorPackageItems
            .FirstOrDefaultAsync(x =>x.VendorPackageItemId == vendorPackageItemId,cancellationToken);
    }
public async Task<VendorPackageItem?>GetByPackageAndServiceIncludingDeletedAsync(Guid vendorPackageId,
           Guid vendorServiceId,CancellationToken cancellationToken = default)
    {
        return await context.VendorPackageItems
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x =>x.VendorPackageId == vendorPackageId && x.VendorServiceId == vendorServiceId,
                cancellationToken);
    }
    public async Task<bool> ExistsAsync(Guid vendorPackageId,Guid vendorServiceId,Guid? excludeVendorPackageItemId,
        CancellationToken cancellationToken = default)
    {
        var query = context.VendorPackageItems
            .Where(x =>x.VendorPackageId ==vendorPackageId &&
                    x.VendorServiceId ==vendorServiceId );

        if (excludeVendorPackageItemId.HasValue)
        {
            query = query.Where(x =>x.VendorPackageItemId !=excludeVendorPackageItemId.Value);
        }
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<PaginatedResult<VendorPackageItem>> GetAllAsync(PaginationRequest request,Guid? vendorPackageId,
        CancellationToken cancellationToken = default)
    {
        var query = context.VendorPackageItems
            .AsNoTracking()
            .AsQueryable();

        if (vendorPackageId.HasValue)
        {
            query = query.Where(x =>x.VendorPackageId ==vendorPackageId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "quantity" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.Quantity)
                    : query.OrderBy(
                        x => x.Quantity),

            "displayorder" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.DisplayOrder)
                    : query.OrderBy(
                        x => x.DisplayOrder),

            _ =>
                query.OrderBy(
                    x => x.DisplayOrder)
        };

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<VendorPackageItem>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}