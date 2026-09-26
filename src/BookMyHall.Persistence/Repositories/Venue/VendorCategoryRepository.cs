using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories;

public sealed class VendorCategoryRepository(BookMyHallDbContext context) : IVendorCategoryRepository
{
    public async Task AddAsync(VendorCategory vendorcategory, CancellationToken cancellationToken = default)
    {
        await context.VendorCategories.AddAsync(vendorcategory, cancellationToken);
    }

    public Task UpdateAsync(VendorCategory vendorcategory, CancellationToken cancellationToken = default)
    {
        context.VendorCategories.Update(vendorcategory);
        return Task.CompletedTask;
    }

    public async Task<VendorCategory?> GetByIdAsync(Guid vendorcategoryId, CancellationToken cancellationToken = default)
    {
        return await context.VendorCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.VendorCategoryId == vendorcategoryId,cancellationToken);
    }

    public async Task<VendorCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.VendorCategories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name && !x.IsDeleted,cancellationToken);
    }

    public async Task<VendorCategory?> GetByNameIncludingDeletedAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.VendorCategories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<PaginatedResult<VendorCategory>> GetAllAsync(PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
       var query = context.VendorCategories
            .AsNoTracking()
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText =request.SearchText.Trim().ToLower();
            query = query.Where(x =>x.Name.ToLower().Contains(searchText)||
                (x.Description != null && x.Description.ToLower().Contains(searchText)));
        }

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => request.SortDescending? query.OrderByDescending(x => x.Name): query.OrderBy(x => x.Name),
            "displayorder" => request.SortDescending ? query.OrderByDescending(x => x.DisplayOrder): query.OrderBy(x => x.DisplayOrder),
            "isactive" => request.SortDescending? query.OrderByDescending(x => x.IsActive): query.OrderBy(x => x.IsActive),
            _ => query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
        };

        var totalCounts = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.PageNumber - 1) *request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<VendorCategory>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCounts
        };
    }
}