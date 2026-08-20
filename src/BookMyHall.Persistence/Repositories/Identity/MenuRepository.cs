using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Persistence.Context;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Persistence.Repositories;

public sealed class MenuRepository(BookMyHallDbContext context)
: IMenuRepository
{
    public async Task<Menu?> GetByIdAsync(
        Guid menuId,
        CancellationToken cancellationToken = default)
        => await context.Menus.FirstOrDefaultAsync(x => x.MenuId == menuId && x.IsActive,
        cancellationToken);

    public async Task<PaginatedResult<Menu>> GetAllAsync(PaginationRequest paginationRequest,
    CancellationToken cancellationToken = default)
    {
        var query = context.Menus.AsNoTracking().Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(paginationRequest.SearchText))
            query = query.Where(x => EF.Functions.ILike(x.MenuName, $"%{paginationRequest.SearchText.Trim()}%"));

        var totalCount = await query.CountAsync(cancellationToken);

        var menus = await query
            .OrderBy(x => x.MenuName)
            .Skip(
                (paginationRequest.PageNumber - 1)
                * paginationRequest.PageSize)
            .Take(
                paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Menu>
        {
            Items = menus,
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber,
            PageSize = paginationRequest.PageSize
        };
    }


    public async Task AddAsync(Menu menu,CancellationToken cancellationToken = default)
    => await context.Menus.AddAsync(menu,cancellationToken);

    public Task UpdateAsync(Menu menu, CancellationToken cancellationToken = default)
    {
        context.Menus.Update(menu);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Menu menu,CancellationToken cancellationToken = default)
    {
        context.Menus.Remove(menu);
        return Task.CompletedTask;
    }
    
}