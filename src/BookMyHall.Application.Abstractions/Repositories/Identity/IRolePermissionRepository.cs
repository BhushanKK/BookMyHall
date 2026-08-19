using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IRolePermissionRepository
{
    Task<RolePermission?> GetByIdAsync(Guid rolePermissionId,CancellationToken cancellationToken = default);

    Task<RolePermission?> GetAsync(Guid roleId,Guid permissionId,CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RolePermission>> GetByRoleIdAsync(Guid roleId,CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync( Guid roleId,Guid permissionId,CancellationToken cancellationToken = default);

    Task AddAsync(RolePermission rolePermission,CancellationToken cancellationToken = default);

    void Delete(RolePermission rolePermission);
}