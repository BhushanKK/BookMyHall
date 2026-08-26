using Microsoft.EntityFrameworkCore;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Persistence.Repositories;
public sealed class HallPricingRepository(BookMyHallDbContext context)
    : IHallPricingRepository
{
    public async Task AddAsync(HallPricing hallPricing,CancellationToken cancellationToken = default)
        => await context.HallPricings.AddAsync(hallPricing, cancellationToken);
    public Task UpdateAsync(HallPricing hallPricing, CancellationToken cancellationToken = default)
    {
        context.HallPricings.Update(hallPricing);
        return Task.CompletedTask;
    }

    public async Task<HallPricing?> GetByIdAsync(Guid hallPricingId,CancellationToken cancellationToken = default)
        => await context.HallPricings
        .Where(x=>x.IsDeleted==false)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.HallPricingId == hallPricingId, cancellationToken);

    public async Task<HallPricing?> GetByHallIdAndEventCategoryIdAsync(
        Guid hallId,
        Guid eventCategoryId,
        CancellationToken cancellationToken = default)
        => await context.HallPricings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.HallId == hallId &&
                     x.EventCategoryId == eventCategoryId,
                cancellationToken);

    public async Task<PaginatedResult<HallPricing>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<HallPricing> query =
            context.HallPricings
            .Where(x=>x.IsDeleted==false)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.PackageName,
                    $"%{search}%"));
        }

        var totalCount =
            await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.PackageName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<HallPricing>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}