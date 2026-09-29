using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories;

public sealed class VendorServiceRepository(
    BookMyHallDbContext context)
    : IVendorServiceRepository
{
    public async Task AddAsync(
        VendorService vendorService,
        CancellationToken cancellationToken = default)
    {
        await context.VendorServices.AddAsync(
            vendorService,
            cancellationToken);
    }

    public Task UpdateAsync(
        VendorService vendorService,
        CancellationToken cancellationToken = default)
    {
        context.VendorServices.Update(
            vendorService);

        return Task.CompletedTask;
    }

    public async Task<VendorService?> GetByIdAsync(
        Guid vendorServiceId,
        CancellationToken cancellationToken = default)
    {
        return await context.VendorServices
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.VendorServiceId == vendorServiceId,
                cancellationToken);
    }

    public async Task<VendorService?> GetByNameAsync(
        Guid vendorId,
        string serviceName,
        CancellationToken cancellationToken = default)
    {
        return await context.VendorServices
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.VendorId == vendorId &&
                    x.ServiceName == serviceName,
                cancellationToken);
    }

    public async Task<VendorService?>
        GetByNameIncludingDeletedAsync(
            Guid vendorId,
            string serviceName,
            CancellationToken cancellationToken = default)
    {
        return await context.VendorServices
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x =>
                    x.VendorId == vendorId &&
                    x.ServiceName == serviceName,
                cancellationToken);
    }

    public async Task<PaginatedResult<VendorService>>
        GetAllAsync(
            PaginationRequest request,
            Guid? vendorId,
            Guid? vendorSubCategoryId,
            CancellationToken cancellationToken = default)
    {
        var query = context.VendorServices
            .AsNoTracking()
            .AsQueryable();

        if (vendorId.HasValue)
        {
            query = query.Where(
                x => x.VendorId == vendorId.Value);
        }

        if (vendorSubCategoryId.HasValue)
        {
            query = query.Where(
                x =>
                    x.VendorSubCategoryId ==
                    vendorSubCategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(
                request.SearchText))
        {
            var searchText =
                request.SearchText.Trim();

            query = query.Where(x =>
                x.ServiceName.Contains(searchText) ||
                (x.Description != null &&
                 x.Description.Contains(searchText)) ||
                (x.PricingType != null &&
                 x.PricingType.Contains(searchText)) ||
                (x.UnitName != null &&
                 x.UnitName.Contains(searchText)));
        }

        var totalCounts = await query.CountAsync(cancellationToken);
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "servicename" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.ServiceName)
                    : query.OrderBy(
                        x => x.ServiceName),

            "pricingtype" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.PricingType)
                    : query.OrderBy(
                        x => x.PricingType),

            "baseprice" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.BasePrice)
                    : query.OrderBy(
                        x => x.BasePrice),

            "minprice" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.MinPrice)
                    : query.OrderBy(
                        x => x.MinPrice),

            "maxprice" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.MaxPrice)
                    : query.OrderBy(
                        x => x.MaxPrice),

            "ispackage" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.IsPackage)
                    : query.OrderBy(
                        x => x.IsPackage),

            "isactive" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.IsActive)
                    : query.OrderBy(
                        x => x.IsActive),

            _ =>
                query.OrderBy(x => x.ServiceName)
        };

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<VendorService>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCounts
        };
    }
      public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(
        string? searchTerm, int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = context.VendorServices.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.ServiceName, pattern));
        }

        return await query
            .OrderBy(x => x.ServiceName)
            .ThenBy(x => x.VendorServiceId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.VendorServiceId, x.ServiceName))
            .ToListAsync(cancellationToken);
    }
}