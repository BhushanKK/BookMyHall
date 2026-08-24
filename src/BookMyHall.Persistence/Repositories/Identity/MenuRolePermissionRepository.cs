using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Infrastructure.Persistence.Repositories;

public sealed class MenuRolePermissionRepository(
    BookMyHallDbContext dbContext)
    : IMenuRolePermissionRepository
{
    public async Task AddAsync(
        MenuRolePermission entity,
        CancellationToken cancellationToken)
    {
        await dbContext.MenuRolePermissions.AddAsync(
            entity,
            cancellationToken);
    }

    public Task UpdateAsync(
        MenuRolePermission entity,
        CancellationToken cancellationToken)
    {
        dbContext.MenuRolePermissions.Update(entity);

        return Task.CompletedTask;
    }

    public async Task<MenuRolePermission?> GetByIdAsync(
        Guid menuRolePermissionId,
        CancellationToken cancellationToken)
    {
        return await dbContext.MenuRolePermissions
            .FirstOrDefaultAsync(
                x => x.MenuRolePermissionId == menuRolePermissionId,
                cancellationToken);
    }

    public async Task<MenuRolePermission?> GetByMenuAndRoleAsync(
        Guid menuId,
        Guid roleId,
        CancellationToken cancellationToken)
    {
        return await dbContext.MenuRolePermissions
            .FirstOrDefaultAsync(
                x =>
                    x.MenuId == menuId &&
                    x.RoleId == roleId,
                cancellationToken);
    }

    public Task DeleteAsync(
        MenuRolePermission entity,
        CancellationToken cancellationToken)
    {
        dbContext.MenuRolePermissions.Remove(entity);

        return Task.CompletedTask;
    }

    public async Task<(
        IReadOnlyList<MenuRolePermission> Items,
        int TotalCount)> GetAllAsync(
        PaginationRequest paginationRequest,
        CancellationToken cancellationToken)
    {
        var query = dbContext.MenuRolePermissions
            .AsNoTracking();

        var totalCount = await query.CountAsync(
            cancellationToken);

        var items = await query
            .OrderBy(x => x.MenuId)
            .ThenBy(x => x.RoleId)
            .Skip(
                (paginationRequest.PageNumber - 1) *
                paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}