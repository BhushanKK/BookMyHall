using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Context;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Persistence.Repositories.Identity;
public sealed class PermissionRepository(BookMyHallDbContext context) : IPermissionRepository
{
    public async Task AddAsync(Permission permission, CancellationToken cancellationToken)
    {
        await context.Permissions.AddAsync(permission, cancellationToken);
    }

    public Task UpdateAsync(Permission permission, CancellationToken cancellationToken)
    {
        context.Permissions.Update(permission);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Permission permission, CancellationToken cancellationToken)
    {
        context.Permissions.Remove(permission);
        return Task.CompletedTask;
    }

    public async Task<Permission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken)
          => await context.Permissions
          .FirstOrDefaultAsync( x => x.PermissionId == permissionId,cancellationToken);
     public async Task<PaginatedResult<Permission>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.Permissions.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.PermissionId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Permission>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}