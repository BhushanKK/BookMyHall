using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class AreaRepository(BookMyHallDbContext context): IAreaRepository
{
    public async Task AddAsync(Area area,CancellationToken cancellationToken = default)
        => await context.Areas.AddAsync(area, cancellationToken);

    public Task UpdateAsync(Area area,CancellationToken cancellationToken = default)
    {
        context.Areas.Update(area);
        return Task.CompletedTask;
    }

    public async Task<Area?> GetByIdAsync(Guid areaId,CancellationToken cancellationToken = default)
        => await context.Areas
            .FirstOrDefaultAsync(x => x.AreaId == areaId && !x.IsDeleted && x.IsActive,cancellationToken);

    public async Task<Area?> GetByAreaNameAsync(string areaName,CancellationToken cancellationToken = default)
        => await context.Areas.AsNoTracking()
            .FirstOrDefaultAsync(x =>x.AreaName == areaName &&!x.IsDeleted,cancellationToken);

    public async Task<PaginatedResult<Area>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var query = context.Areas.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => 
            EF.Functions.ILike(x.AreaName, pattern) || EF.Functions.ILike(x.Pincode, pattern));
        }

         var totalCount = await query.CountAsync(cancellationToken);
         var items = await query
            .OrderBy(x => x.AreaName)
            .ThenBy(x => x.AreaId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Area>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}