using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Repositories;

public sealed class RoleRepository(BookMyHallDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default)
       => await context.Roles.FirstOrDefaultAsync(x => x.RoleId == roleId && x.IsActive,
        cancellationToken);

    public async Task<PaginatedResult<Role>> GetAllAsync(
    PaginationRequest paginationRequest,
    CancellationToken cancellationToken = default)
    {
        var query = context.Roles.AsNoTracking().Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(paginationRequest.SearchText))
            query = query.Where(x => x.RoleName.Contains(paginationRequest.SearchText));

        var totalCount = await query.CountAsync(cancellationToken);

        var roles = await query
            .OrderBy(x => x.RoleName)
            .Skip(
                (paginationRequest.PageNumber - 1)
                * paginationRequest.PageSize)
            .Take(
                paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Role>
        {
            Items = roles,
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber,
            PageSize = paginationRequest.PageSize
        };
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        => await context.Roles.AddAsync(role, cancellationToken);

    public Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        context.Roles.Update(role);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Role role, CancellationToken cancellationToken = default)
    {
        context.Roles.Remove(role);
        return Task.CompletedTask;
    }
}