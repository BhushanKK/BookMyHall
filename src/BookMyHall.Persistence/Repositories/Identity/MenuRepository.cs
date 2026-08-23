using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Persistence.Context;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Persistence.Repositories;

public sealed class MenuRepository(BookMyHallDbContext context)
: IMenuRepository
{
    public async Task<Menu?> GetByIdAsync(Guid menuId,CancellationToken cancellationToken = default)
        => await context.Menus.FirstOrDefaultAsync(x => x.MenuId == menuId && x.IsActive, cancellationToken);

    public async Task<IReadOnlyList<Menu>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Menus
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Level)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.MenuName)
            .ToListAsync(cancellationToken);
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