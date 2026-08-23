using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class CountryRepository(BookMyHallDbContext context)
    : ICountryRepository
{
    public async Task AddAsync(Country country,CancellationToken cancellationToken = default)
        => await context.Countries.AddAsync(country, cancellationToken);

    public Task UpdateAsync(Country country,CancellationToken cancellationToken = default)
    {
        context.Countries.Update(country);
        return Task.CompletedTask;
    }

    public async Task<Country?> GetByIdAsync(Guid countryId,CancellationToken cancellationToken = default)
        => await context.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CountryId == countryId,
                cancellationToken);

    public async Task<Country?> GetByCountryNameAsync( string countryName,CancellationToken cancellationToken = default)
        => await context.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CountryName == countryName,
                cancellationToken);

    public async Task<PaginatedResult<Country>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Country> query =
            context.Countries.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.CountryName,
                    $"%{search}%"));
        }

        var totalCount =
            await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.CountryName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Country>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    
}