using Microsoft.EntityFrameworkCore;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Infrastructure.Persistence.Repositories;

public sealed class MenuRolePermissionRepository(
    BookMyHallDbContext dbContext)
    : IMenuRolePermissionRepository
{
    public async Task<IReadOnlyList<MenuRolePermission>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken)
    {
        return await dbContext.MenuRolePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertRangeAsync(
        IReadOnlyList<MenuRolePermission> entities,
        CancellationToken cancellationToken)
    {
        if (entities.Count == 0)
        {
            return;
        }

        var roleId = entities[0].RoleId;

        var menuIds = entities
            .Select(x => x.MenuId)
            .Distinct()
            .ToList();

        var existingPermissions =
            await dbContext.MenuRolePermissions
                .Where(x =>
                    x.RoleId == roleId &&
                    menuIds.Contains(x.MenuId))
                .ToListAsync(cancellationToken);

        var existingLookup =
            existingPermissions.ToDictionary(
                x => x.MenuId);

        foreach (var entity in entities)
        {
            if (existingLookup.TryGetValue(
                    entity.MenuId,
                    out var existing))
            {
                existing.CanView = entity.CanView;
                existing.CanCreate = entity.CanCreate;
                existing.CanUpdate = entity.CanUpdate;
                existing.CanDelete = entity.CanDelete;
                existing.CanPrint = entity.CanPrint;
                existing.CanExport = entity.CanExport;
            }
            else
            {
                await dbContext.MenuRolePermissions.AddAsync(
                    entity,
                    cancellationToken);
            }
        }
    }
}