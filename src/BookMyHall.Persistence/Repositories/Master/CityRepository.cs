using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class CityRepository(BookMyHallDbContext context)
    : ICityRepository
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
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CityId == cityId,
                cancellationToken);

    public async Task<City?> GetByCityNameAsync(string cityName,CancellationToken cancellationToken = default)
        => await context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CityName == cityName,
                cancellationToken);

    public async Task<PaginatedResult<City>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        IQueryable<City> query = context.Cities.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.CityName, $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.CityName)
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
}