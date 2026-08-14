using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface ICountryRepository

{
    Task AddAsync(Country country,CancellationToken cancellationToken=default);
    Task UpdateAsync(Country country,CancellationToken cancellationToken=default);
    Task<Country?> GetByIdAsync( Guid countryId, CancellationToken cancellationToken = default);
    Task<Country?> GetByCountryNameAsync( string countryName,CancellationToken cancellationToken = default);
    Task<PaginatedResult<Country>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
}