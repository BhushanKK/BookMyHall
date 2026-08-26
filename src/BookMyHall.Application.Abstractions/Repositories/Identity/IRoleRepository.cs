using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Role>> GetAllAsync(PaginationRequest paginationRequest, 
    CancellationToken cancellationToken = default);
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    Task UpdateAsync(Role role, CancellationToken cancellationToken = default);
}