using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid permissionId,CancellationToken cancellationToken = default);
    Task AddAsync(Permission permission,CancellationToken cancellationToken);
    Task UpdateAsync(Permission permission,CancellationToken cancellationToken);
    Task DeleteAsync (Permission permission,CancellationToken cancellationToken);
    Task<PaginatedResult<Permission>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken= default);
}