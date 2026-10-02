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
            .FirstOrDefaultAsync(x => x.VendorCategoryId == vendorcategoryId && !x.IsDeleted,cancellationToken);
    }

    public async Task<VendorCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.VendorCategories
            .FirstOrDefaultAsync(x => x.VendorCategoryName == name && !x.IsDeleted,cancellationToken);
    }

    public async Task<VendorCategory?> GetByNameIncludingDeletedAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.VendorCategories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.VendorCategoryName == name, cancellationToken);
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
            query = query.Where(x =>x.VendorCategoryName.ToLower().Contains(searchText)||
                (x.Description != null && x.Description.ToLower().Contains(searchText)));
        }

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => request.SortDescending? query.OrderByDescending(x => x.VendorCategoryName): query.OrderBy(x => x.VendorCategoryName),
            "displayorder" => request.SortDescending ? query.OrderByDescending(x => x.DisplayOrder): query.OrderBy(x => x.DisplayOrder),
            "isactive" => request.SortDescending? query.OrderByDescending(x => x.IsActive): query.OrderBy(x => x.IsActive),
            _ => query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.VendorCategoryName)
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
      public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(
        string? searchTerm, int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = context.VendorCategories.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.VendorCategoryName, pattern));
        }

        return await query
            .OrderBy(x => x.VendorCategoryName)
            .ThenBy(x => x.VendorCategoryId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.VendorCategoryId, x.VendorCategoryName))
            .ToListAsync(cancellationToken);
    }
}