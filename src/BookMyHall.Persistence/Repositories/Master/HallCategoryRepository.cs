using Microsoft.EntityFrameworkCore;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Persistence.Repositories;

public sealed class HallCategoryRepository(BookMyHallDbContext context): IHallCategoryRepository
{
     public async Task AddAsync(HallCategory hallCategory,CancellationToken cancellationToken = default)
        => await context.HallCategories.AddAsync(hallCategory, cancellationToken);
    public Task UpdateAsync(HallCategory hallCategory, CancellationToken cancellationToken = default)
    {
        context.HallCategories.Update(hallCategory);
        return Task.CompletedTask;
    }
    public async Task<HallCategory?> GetByIdAsync(Guid hallCategoryId,CancellationToken cancellationToken = default)
        => await context.HallCategories
        .FirstOrDefaultAsync(x => x.HallCategoryId == hallCategoryId && !x.IsDeleted && x.IsActive,cancellationToken);

    public async Task<PaginatedResult<HallCategory>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.HallCategories.AsNoTracking().Where(x=>!x.IsDeleted && x.IsActive);
         if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern=$"%{search}%";
            query = query.Where(x=>EF.Functions.ILike( x.HallCategoryName,pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.HallCategoryName)
            .ThenBy(x=>x.HallCategoryId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<HallCategory>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
    public async Task<HallCategory?> GetByNameIncludingDeletedAsync(string hallcategoryName, CancellationToken cancellationToken)
    {
        var normalizedName = hallcategoryName.Trim();
        return await context.HallCategories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.HallCategoryName == normalizedName, cancellationToken);
    }

    public async Task<HallCategory?> GetByHallCategoryNameAsync(string hallCategoryName,CancellationToken cancellationToken = default)
        => await context.HallCategories.AsNoTracking()
            .FirstOrDefaultAsync( x => x.HallCategoryName == hallCategoryName &&!x.IsDeleted,cancellationToken);


    public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, 
        int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = context.HallCategories.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.HallCategoryName, pattern));
        }

        return await query
            .OrderBy(x => x.HallCategoryName)
            .ThenBy(x => x.HallCategoryId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.HallCategoryId, x.HallCategoryName))
            .ToListAsync(cancellationToken);
    }
}