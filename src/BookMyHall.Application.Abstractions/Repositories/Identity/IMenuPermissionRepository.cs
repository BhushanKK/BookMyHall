using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IMenuPermissionRepository
{
    Task<MenuPermission?> GetByIdAsync(Guid menuPermissionId,CancellationToken cancellationToken = default);
    Task<PaginatedResult<MenuPermission>> GetAllAsync(PaginationRequest paginationRequest,CancellationToken cancellationToken = default);
    Task<MenuPermission?> GetAsync(Guid menuId,Guid permissionId,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuPermission>> GetByMenuIdAsync(Guid menuId,CancellationToken cancellationToken = default);
    Task AddAsync(MenuPermission menuPermission,CancellationToken cancellationToken = default);
    Task DeleteAsync(MenuPermission menuPermission,CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuPermission menuPermission,CancellationToken cancellationToken = default);

}