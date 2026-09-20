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
        => await context.Districts
            .FirstOrDefaultAsync(x => x.DistrictId == districtId && !x.IsDeleted && x.IsActive,cancellationToken);

    public async Task<District?> GetByDistrictNameAsync(string districtName,CancellationToken cancellationToken = default)
        => await context.Districts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.DistrictName == districtName && !x.IsDeleted,cancellationToken);

    public async Task<PaginatedResult<District>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.Districts.AsNoTracking().Where(x=>!x.IsDeleted && x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern=$"%{search}%";
            query = query.Where(x =>EF.Functions.ILike(x.DistrictName,pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.DistrictName)
            .ThenBy(x=>x.DistrictId)
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
     public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, 
        int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Districts.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.DistrictName, pattern));
        }

        return await query
            .OrderBy(x => x.DistrictName)
            .ThenBy(x => x.DistrictId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.DistrictId, x.DistrictName))
            .ToListAsync(cancellationToken);
    }
}