using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IAmenityRepository
{
    Task AddAsync(Amenity amenity, CancellationToken cancellationToken = default);

    Task UpdateAsync(Amenity amenity, CancellationToken cancellationToken = default);

    Task<Amenity?> GetByIdAsync(Guid amenityId, CancellationToken cancellationToken = default);

    Task<Amenity?> GetByAmenityNameAsync(string amenityName, CancellationToken cancellationToken = default);

    Task<PaginatedResult<Amenity>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default);
}