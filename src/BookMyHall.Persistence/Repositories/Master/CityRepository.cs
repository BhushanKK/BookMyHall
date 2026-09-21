using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class CityRepository(BookMyHallDbContext context): ICityRepository
{
    public async Task AddAsync(City city,CancellationToken cancellationToken = default)
        => await context.Cities.AddAsync(city, cancellationToken);

    public Task UpdateAsync(City city,CancellationToken cancellationToken = default)
    {
        context.Cities.Update(city);
        return Task.CompletedTask;
    }

    public async Task<City?> GetByIdAsync(Guid cityId,CancellationToken cancellationToken = default)
        => await context.Cities
            .FirstOrDefaultAsync(x => x.CityId == cityId  && !x.IsDeleted && x.IsActive,cancellationToken);

    public async Task<City?> GetByCityNameAsync(string cityName,CancellationToken cancellationToken = default)
        => await context.Cities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CityName == cityName && !x.IsDeleted,cancellationToken);

    public async Task<PaginatedResult<City>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.Cities.AsNoTracking().Where(x=>!x.IsDeleted && x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x =>EF.Functions.ILike(x.CityName, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.CityName)
            .ThenBy(x=>x.CityId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<City>
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
        var query = context.Cities.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.CityName, pattern));
        }

        return await query
            .OrderBy(x => x.CityName)
            .ThenBy(x => x.CityId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.CityId, x.CityName))
            .ToListAsync(cancellationToken);
    }
}