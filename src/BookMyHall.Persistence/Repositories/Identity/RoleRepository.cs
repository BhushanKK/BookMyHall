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
       => await context.Roles.FirstOrDefaultAsync(x => x.RoleId == roleId,
        cancellationToken);

    public async Task<PaginatedResult<Role>> GetAllAsync(
    PaginationRequest paginationRequest,
    CancellationToken cancellationToken = default)
    {
        var query = context.Roles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(paginationRequest.SearchText))
        {
            var searchText = paginationRequest.SearchText.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.RoleName,
                    $"%{searchText}%"));
        }

        var totalCount = await query.CountAsync(
            cancellationToken);

        query = paginationRequest.SortDescending
            ? query.OrderByDescending(x => x.RoleName)
            : query.OrderBy(x => x.RoleName);

        var roles = await query
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