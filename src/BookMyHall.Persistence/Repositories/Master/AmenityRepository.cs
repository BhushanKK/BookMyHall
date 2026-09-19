using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class AmenityRepository(BookMyHallDbContext context) : IAmenityRepository
{
    public async Task AddAsync(Amenity amenity,CancellationToken cancellationToken = default)
        => await context.Amenitys.AddAsync(amenity, cancellationToken);

    public Task UpdateAsync(Amenity amenity,CancellationToken cancellationToken = default)
    {
        context.Amenitys.Update(amenity);
        return Task.CompletedTask;
    }

    public async Task<Amenity?> GetByIdAsync(Guid amenityId,CancellationToken cancellationToken = default)
        => await context.Amenitys
            .FirstOrDefaultAsync(x => x.AmenityId == amenityId && !x.IsDeleted && x.IsActive,cancellationToken);

    public async Task<Amenity?> GetByAmenityNameAsync(string amenityName,CancellationToken cancellationToken = default)
        => await context.Amenitys.AsNoTracking()
            .FirstOrDefaultAsync(x => x.AmenityName == amenityName && !x.IsDeleted, cancellationToken);

    public async Task<PaginatedResult<Amenity>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.Amenitys.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern= $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.AmenityName,pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.AmenityName)
            .ThenBy(x => x.AmenityId)
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

    public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(
        string? searchTerm, int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = context.Amenitys.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.AmenityName, pattern));
        }

        return await query
            .OrderBy(x => x.AmenityName)
            .ThenBy(x => x.AmenityId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.AmenityId, x.AmenityName))
            .ToListAsync(cancellationToken);
    }
}