using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IHallRepository
{
    Task AddAsync(Hall hall, CancellationToken cancellationToken = default);
    Task UpdateAsync(Hall hall, CancellationToken cancellationToken = default);
    Task<Hall?> GetByIdAsync(Guid hallId, CancellationToken cancellationToken = default);
    Task<Hall?> GetByHallNameAndAreaAsync(string hallName,Guid areaId,CancellationToken cancellationToken = default);
    Task<PaginatedResult<Hall>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
}