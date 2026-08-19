using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories;

public sealed class RolePermissionRepository(BookMyHallDbContext context) : IRolePermissionRepository
{
    public async Task<RolePermission?> GetByIdAsync(Guid rolePermissionId, CancellationToken cancellationToken = default)
    {
        return await context.RolePermissions
            .FirstOrDefaultAsync(x => x.RolePermissionId == rolePermissionId, cancellationToken);
    }

    public async Task<RolePermission?> GetAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        return await context.RolePermissions
            .FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId, cancellationToken);
    }

    public async Task<IReadOnlyList<RolePermission>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await context.RolePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        return await context.RolePermissions
            .AnyAsync(x => x.RoleId == roleId && x.PermissionId == permissionId, cancellationToken);
    }

    public async Task AddAsync(RolePermission rolePermission, CancellationToken cancellationToken = default)
       => await context.RolePermissions.AddAsync(rolePermission, cancellationToken);

   public void Delete(RolePermission rolePermission)
    {
        context.RolePermissions.Remove(rolePermission);
    }

}