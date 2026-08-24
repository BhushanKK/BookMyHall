using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IMenuRolePermissionRepository
{
    Task AddAsync(MenuRolePermission entity, CancellationToken cancellationToken);
    Task UpdateAsync(MenuRolePermission entity, CancellationToken cancellationToken);
    Task<MenuRolePermission?> GetByIdAsync(Guid menuRolePermissionId, CancellationToken cancellationToken);
    Task<MenuRolePermission?> GetByMenuAndRoleAsync(Guid menuId, Guid roleId, CancellationToken cancellationToken);
    Task DeleteAsync(MenuRolePermission entity, CancellationToken cancellationToken);
    Task<(IReadOnlyList<MenuRolePermission> Items, int TotalCount)> GetAllAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken);
}