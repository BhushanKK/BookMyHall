using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Persistence.Context;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Persistence.Repositories;

public sealed class MenuPermissionRepository(BookMyHallDbContext context)
    : IMenuPermissionRepository
{
    public async Task<MenuPermission?> GetByIdAsync(
        Guid menuPermissionId,
        CancellationToken cancellationToken = default)
        => await context.MenuPermissions
            .FirstOrDefaultAsync(
                x => x.MenuPermissionId == menuPermissionId,
                cancellationToken);

    public async Task<PaginatedResult<MenuPermission>> GetAllAsync(
        PaginationRequest paginationRequest,
        CancellationToken cancellationToken = default)
    {
        var query = context.MenuPermissions
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var menuPermissions = await query
            .OrderBy(x => x.MenuPermissionId)
            .Skip(
                (paginationRequest.PageNumber - 1)
                * paginationRequest.PageSize)
            .Take(
                paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<MenuPermission>
        {
            Items = menuPermissions,
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber,
            PageSize = paginationRequest.PageSize
        };
    }

    public async Task<IReadOnlyList<MenuPermission>> GetByMenuIdAsync( Guid menuId,
        CancellationToken cancellationToken = default)
        => await context.MenuPermissions
            .AsNoTracking()
            .Where(x => x.MenuId == menuId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(MenuPermission menuPermission,CancellationToken cancellationToken = default)
        => await context.MenuPermissions.AddAsync(
            menuPermission,
            cancellationToken);

    public Task UpdateAsync(MenuPermission menuPermission,CancellationToken cancellationToken = default)
    {
        context.MenuPermissions.Update(menuPermission);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(MenuPermission menuPermission,CancellationToken cancellationToken = default)
    {
        context.MenuPermissions.Remove(menuPermission);
        return Task.CompletedTask;
    }

    public async Task<MenuPermission?> GetAsync(Guid menuId,Guid permissionId,
    CancellationToken cancellationToken = default)
    => await context.MenuPermissions
        .FirstOrDefaultAsync(
            x => x.MenuId == menuId &&
                 x.PermissionId == permissionId,
            cancellationToken);
}