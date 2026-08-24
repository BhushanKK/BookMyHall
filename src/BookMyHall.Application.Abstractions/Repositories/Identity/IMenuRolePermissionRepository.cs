using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IMenuRolePermissionRepository
{
    Task<IReadOnlyList<MenuRolePermission>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken);

    Task UpsertRangeAsync(
        IReadOnlyList<MenuRolePermission> entities,
        CancellationToken cancellationToken);
}