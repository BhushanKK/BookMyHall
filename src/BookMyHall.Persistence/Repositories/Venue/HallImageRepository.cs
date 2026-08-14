using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
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

    public async Task<IReadOnlyList<HallImage>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default)
    {
        return await context.HallImages
            .AsNoTracking()
            .Where(x =>
                x.HallId == hallId &&
                x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
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