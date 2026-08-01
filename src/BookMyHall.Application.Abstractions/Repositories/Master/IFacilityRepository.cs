using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IFacilityRepository
{
    Task AddAsync(Facility facility,CancellationToken cancellationToken = default);

    Task UpdateAsync(Facility facility,CancellationToken cancellationToken = default);

    Task<Facility?> GetByIdAsync(Guid facilityId,CancellationToken cancellationToken = default);

    Task<Facility?> GetByFacilityNameAsync(string facilityName,CancellationToken cancellationToken = default);

    Task<PaginatedResult<Facility>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
}