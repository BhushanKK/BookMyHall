using Microsoft.EntityFrameworkCore;

using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class HallRepository(BookMyHallDbContext context) : IHallRepository
{
    public async Task AddAsync(Hall hall,CancellationToken cancellationToken = default)
        => await context.Halls.AddAsync(hall, cancellationToken);

    public Task UpdateAsync(Hall hall,CancellationToken cancellationToken = default)
    {
        context.Halls.Update(hall);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Hall hall,CancellationToken cancellationToken = default)
    {
        context.Halls.Remove(hall);
        return Task.CompletedTask;
    }
    public async Task<Hall?> GetByIdAsync(Guid hallId,CancellationToken cancellationToken = default)
        => await context.Halls.AsNoTracking().FirstOrDefaultAsync(x => x.HallId == hallId,cancellationToken);
    public async Task<Hall?> GetByHallNameAndAreaAsync(
    string hallName,
    Guid areaId,
    CancellationToken cancellationToken = default)
    => await context.Halls
        .AsNoTracking()
        .FirstOrDefaultAsync(
            x =>
                x.HallName == hallName &&
                x.AreaId == areaId,
            cancellationToken);

    public async Task<PaginatedResult<Hall>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Hall> query = context.Halls.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.HallName,
                    $"%{search}%"));
        }

        var totalCount = await query.CountAsync(
            cancellationToken);

        var items = await query
            .OrderBy(x => x.HallName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Hall>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}