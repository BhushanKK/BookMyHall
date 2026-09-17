using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class EventCategoryRepository(BookMyHallDbContext context): IEventCategoryRepository
{
    public async Task AddAsync(EventCategory eventCategory,CancellationToken cancellationToken = default)
        => await context.EventCategories.AddAsync(eventCategory,cancellationToken);

    public Task UpdateAsync(EventCategory eventCategory,CancellationToken cancellationToken = default)
    {
        context.EventCategories.Update(eventCategory);
        return Task.CompletedTask;
    }

    public async Task<EventCategory?> GetByIdAsync(Guid eventCategoryId,CancellationToken cancellationToken = default)
        => await context.EventCategories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EventCategoryId == eventCategoryId && !x.IsDeleted && x.IsActive,cancellationToken);

    public async Task<EventCategory?> GetByEventCategoryNameAsync(string eventCategoryName,CancellationToken cancellationToken = default)
        => await context.EventCategories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EventCategoryName == eventCategoryName && !x.IsDeleted ,cancellationToken);

    public async Task<PaginatedResult<EventCategory>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.EventCategories.AsNoTracking().Where(x=>x.IsDeleted==false && x.IsActive==true);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern=$"%{search}%";
            query = query.Where(x =>EF.Functions.ILike(x.EventCategoryName,pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.EventCategoryName)
            .ThenBy(x=>x.EventCategoryId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<EventCategory>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}