using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories;
public sealed class VendorSubCategoryRepository(BookMyHallDbContext context): IVendorSubCategoryRepository
{
    public async Task AddAsync(VendorSubCategory vendorSubCategory,
        CancellationToken cancellationToken = default)
    {
        await context.VendorSubCategories.AddAsync(vendorSubCategory,cancellationToken);
    }

    public Task UpdateAsync(VendorSubCategory vendorSubCategory,CancellationToken cancellationToken = default)
    {
        context.VendorSubCategories.Update(vendorSubCategory);
        return Task.CompletedTask;
    }

    public async Task<VendorSubCategory?> GetByIdAsync(Guid vendorSubCategoryId,
        CancellationToken cancellationToken = default)
    {
        return await context.VendorSubCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>x.VendorSubCategoryId ==vendorSubCategoryId,cancellationToken);
    }

    public async Task<VendorSubCategory?> GetByNameAsync(Guid vendorCategoryId,string name,
        CancellationToken cancellationToken = default)
    {
        return await context.VendorSubCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>x.VendorCategoryId ==vendorCategoryId &&x.Name == name,cancellationToken);
    }

    public async Task<VendorSubCategory?>GetByNameIncludingDeletedAsync(Guid vendorCategoryId,string name,
            CancellationToken cancellationToken = default)
    {
        return await context.VendorSubCategories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x =>x.VendorCategoryId ==vendorCategoryId &&x.Name == name,cancellationToken);
    }

    public async Task<PaginatedResult<VendorSubCategory>>GetAllAsync(PaginationRequest request,
    Guid? vendorCategoryId,CancellationToken cancellationToken = default)
    {
        var query = context.VendorSubCategories
            .AsNoTracking()
            .AsQueryable();

        if (vendorCategoryId.HasValue)
        {
            query = query.Where(x =>x.VendorCategoryId ==vendorCategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim();
            query = query.Where(x => x.Name.Contains(searchText) ||
                (x.Description != null && x.Description.Contains(searchText)));
        }

        var totalCounts = await query.CountAsync(cancellationToken);

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "name" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.Name)
                    : query.OrderBy(
                        x => x.Name),

            "description" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.Description)
                    : query.OrderBy(
                        x => x.Description),

            "displayorder" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.DisplayOrder)
                    : query.OrderBy(
                        x => x.DisplayOrder),

            "isactive" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.IsActive)
                    : query.OrderBy(
                        x => x.IsActive),

            _ =>
                query
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.Name)
        };

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<VendorSubCategory>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCounts
        };
    }
}