using Microsoft.EntityFrameworkCore;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Persistence.Repositories;


public sealed class HallCategoryRepository(BookMyHallDbContext context): IHallCategoryRepository
{
    public async Task<HallCategory?> GetByIdAsync(Guid hallCategoryId,CancellationToken cancellationToken = default)
    {
        return await context.HallCategories
            .FirstOrDefaultAsync(x => x.HallCategoryId == hallCategoryId,cancellationToken);
    }

    public async Task<PaginatedResult<HallCategory>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.HallCategories
            .AsNoTracking()
            .Where(x => x.IsActive);

        var totalCount = await query.CountAsync(
            cancellationToken);

        var items = await query
            .OrderBy(x => x.HallCategoryName)
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

    public async Task AddAsync(HallCategory hallCategory,CancellationToken cancellationToken = default)
      =>  await context.HallCategories.AddAsync(hallCategory, cancellationToken);

    public Task UpdateAsync(HallCategory hallCategory, CancellationToken cancellationToken = default)
    {
        context.HallCategories.Update(hallCategory);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(HallCategory hallCategory, CancellationToken cancellationToken = default)
    {
        context.HallCategories.Remove(hallCategory);
        return Task.CompletedTask;
    }
}