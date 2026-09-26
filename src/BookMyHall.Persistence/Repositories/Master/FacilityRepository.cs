using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class FacilityRepository(BookMyHallDbContext context): IFacilityRepository
{
    public async Task AddAsync(Facility facility,CancellationToken cancellationToken = default)
        => await context.Facilities.AddAsync(facility,cancellationToken);

    public Task UpdateAsync(Facility facility,CancellationToken cancellationToken = default)
    {
        context.Facilities.Update(facility);
        return Task.CompletedTask;
    }

    public async Task<Facility?> GetByIdAsync(Guid facilityId,CancellationToken cancellationToken = default)
        => await context.Facilities
        .FirstOrDefaultAsync(x => x.FacilityId == facilityId && !x.IsDeleted && x.IsActive,cancellationToken);
    public async Task<Facility?> GetByNameIncludingDeletedAsync(string facilityName, CancellationToken cancellationToken)
    {
        var normalizedName = facilityName.Trim();
        return await context.Facilities
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.FacilityName == normalizedName, cancellationToken);
    }
    public async Task<Facility?> GetByFacilityNameAsync(string facilityName,CancellationToken cancellationToken = default)
        => await context.Facilities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.FacilityName == facilityName && !x.IsDeleted,cancellationToken);

    public async Task<PaginatedResult<Facility>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.Facilities.AsNoTracking().Where(x=>!x.IsDeleted && x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern=$"%{search}%";
            query = query.Where(x =>EF.Functions.ILike(x.FacilityName,pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.FacilityName)
            .ThenBy(x=>x.FacilityId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Facility>
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
        var query = context.Facilities.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.FacilityName, pattern));
        }

        return await query
            .OrderBy(x => x.FacilityName)
            .ThenBy(x => x.FacilityId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.FacilityId, x.FacilityName))
            .ToListAsync(cancellationToken);
    }
}