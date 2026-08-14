using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories.Venue;

public sealed class HallImageRepository(BookMyHallDbContext context)
    : IHallImageRepository
{
    public async Task<HallImage?> GetByIdAsync(Guid hallImageId, CancellationToken cancellationToken = default)
    {
        return await context.HallImages
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.HallImageId == hallImageId,
                cancellationToken);
    }
    public async Task<PaginatedResult<HallImage>> GetByHallIdAsync(
    Guid hallId,
    PaginationRequest request,
    CancellationToken cancellationToken = default)
    {
        IQueryable<HallImage> query = context.HallImages
            .AsNoTracking()
            .Where(x =>
                x.HallId == hallId &&
                x.IsActive);

        var totalCount = await query.CountAsync(
            cancellationToken);

        var items = await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.HallImageId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<HallImage>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<HallImage?> GetCoverImageAsync(Guid hallId, CancellationToken cancellationToken = default)
    {
        return await context.HallImages
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.HallId == hallId &&
                    x.IsCoverImage &&
                    x.IsActive,
                cancellationToken);
    }
    public async Task AddAsync(HallImage hallImage, CancellationToken cancellationToken = default)
        => await context.HallImages.AddAsync(hallImage, cancellationToken);
    public Task UpdateAsync(HallImage hallImage, CancellationToken cancellationToken = default)
    {
        context.HallImages.Update(hallImage);
        return Task.CompletedTask;
    }
    public Task DeleteAsync(HallImage hallImage, CancellationToken cancellationToken = default)
    {
        context.HallImages.Remove(hallImage);
        return Task.CompletedTask;
    }
}