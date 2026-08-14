using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Common.Interfaces.Repositories.Venue;

public interface IHallImageRepository
{
    Task<HallImage?> GetByIdAsync(Guid hallImageId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HallImage>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default);
    Task<HallImage?> GetCoverImageAsync(Guid hallId, CancellationToken cancellationToken = default);
    Task AddAsync(HallImage hallImage, CancellationToken cancellationToken = default);
    Task UpdateAsync(HallImage hallImage, CancellationToken cancellationToken = default);
    Task DeleteAsync(HallImage hallImage, CancellationToken cancellationToken = default);
}