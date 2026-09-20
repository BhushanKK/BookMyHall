using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IAreaRepository
{
    Task AddAsync(Area area, CancellationToken cancellationToken = default);

    Task UpdateAsync(Area area, CancellationToken cancellationToken = default);

    Task<Area?> GetByIdAsync(Guid areaId, CancellationToken cancellationToken = default);

    Task<Area?> GetByAreaNameAsync(string areaName, CancellationToken cancellationToken = default);

    Task<PaginatedResult<Area>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, int limit = 20, CancellationToken cancellationToken = default);
}