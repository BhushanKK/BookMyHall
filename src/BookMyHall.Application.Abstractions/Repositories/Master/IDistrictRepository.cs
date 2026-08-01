using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IDistrictRepository
{
    Task AddAsync(District district,CancellationToken cancellationToken = default);

    Task UpdateAsync(District district,CancellationToken cancellationToken = default);

    Task<District?> GetByIdAsync(Guid districtId,CancellationToken cancellationToken = default);

    Task<District?> GetByDistrictNameAsync(string districtName,CancellationToken cancellationToken = default);

    Task<PaginatedResult<District>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
}