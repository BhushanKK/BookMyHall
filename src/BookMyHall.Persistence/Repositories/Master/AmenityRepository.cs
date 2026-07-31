using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class AmenityRepository(BookMyHallDbContext context)
    : IAmenityRepository
{
    public async Task AddAsync(
        Amenity amenity,
        CancellationToken cancellationToken = default)
        => await context.Amenitys.AddAsync(amenity, cancellationToken);

    public Task UpdateAsync(
        Amenity amenity,
        CancellationToken cancellationToken = default)
    {
        context.Amenitys.Update(amenity);
        return Task.CompletedTask;
    }

    public async Task<Amenity?> GetByIdAsync(
        Guid amenityId,
        CancellationToken cancellationToken = default)
        => await context.Amenitys
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.AmenityId == amenityId,
                cancellationToken);

    public async Task<Amenity?> GetByAmenityNameAsync(
        string amenityName,
        CancellationToken cancellationToken = default)
        => await context.Amenitys
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.AmenityName == amenityName,
                cancellationToken);

    public async Task<PaginatedResult<Amenity>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Amenity> query = context.Amenitys.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.AmenityName, $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.AmenityName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Amenity>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}