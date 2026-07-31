using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface ICityRepository
{
    Task AddAsync( City city,CancellationToken cancellationToken = default);

    Task UpdateAsync( City city,CancellationToken cancellationToken = default);

    Task<City?> GetByIdAsync(Guid cityId,CancellationToken cancellationToken = default);

    Task<City?> GetByCityNameAsync(string cityName,CancellationToken cancellationToken = default);

    Task<PaginatedResult<City>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
}