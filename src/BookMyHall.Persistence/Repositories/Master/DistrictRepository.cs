using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class DistrictRepository(BookMyHallDbContext context):IDistrictRepository
{
    public async Task AddAsync(District district,CancellationToken cancellationToken = default)
        => await context.Districts.AddAsync(district,cancellationToken);

    public Task UpdateAsync(District district,CancellationToken cancellationToken = default)
    {
        context.Districts.Update(district);
        return Task.CompletedTask;
    }

    public async Task<District?> GetByIdAsync(Guid districtId,CancellationToken cancellationToken = default)
        => await context.Districts.Where(x=>x.IsDeleted==false)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DistrictId == districtId,cancellationToken);

    public async Task<District?> GetByDistrictNameAsync(string districtName,CancellationToken cancellationToken = default)
        => await context.Districts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DistrictName == districtName,cancellationToken);

    public async Task<PaginatedResult<District>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        IQueryable<District> query = context.Districts
        .Where(x=>x.IsDeleted==false)
        .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(
                    x.DistrictName,
                    $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.DistrictName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<District>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}