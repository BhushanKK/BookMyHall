using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IStateRepository
{
    Task AddAsync(State state, CancellationToken cancellationToken = default);

    Task UpdateAsync(State state, CancellationToken cancellationToken = default);

    Task<State?> GetByIdAsync(Guid stateId, CancellationToken cancellationToken = default);

    Task<State?> GetByStateCodeAsync(string stateCode, CancellationToken cancellationToken = default);

    Task<State?> GetByStateNameAsync(string stateName, CancellationToken cancellationToken = default);
     Task<PaginatedResult<State>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);

}