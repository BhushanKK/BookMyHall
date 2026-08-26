using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class HallBlockRepository(BookMyHallDbContext context): IHallBlockRepository
{
    public async Task<HallBlock?> GetByIdAsync(Guid hallBlockId,CancellationToken cancellationToken = default)
    {
        return await context.HallBlocks
        .Where(x=>x.IsDeleted==false)
            .FirstOrDefaultAsync( x => x.HallBlockId == hallBlockId,cancellationToken);
    }

    public async Task<PaginatedResult<HallBlock>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.HallBlocks
        .Where(x=>x.IsDeleted==false)
            .AsNoTracking()
            .Where(x => x.IsActive);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.BlockFromDate)
            .ThenBy(x => x.StartTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<HallBlock>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task AddAsync(HallBlock hallBlock,CancellationToken cancellationToken = default)
      =>  await context.HallBlocks.AddAsync(hallBlock,cancellationToken);

    public Task UpdateAsync(HallBlock hallBlock,CancellationToken cancellationToken = default)
    {
        context.HallBlocks.Update(hallBlock);
        return Task.CompletedTask;
    }
}