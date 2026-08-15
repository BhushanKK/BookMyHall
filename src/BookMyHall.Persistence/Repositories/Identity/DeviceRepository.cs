using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Context;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Persistence.Repositories;

public sealed class DeviceRepository(BookMyHallDbContext context)
    : IDeviceRepository
{
    public async Task<Device?> GetByDeviceIdentifierAsync(Guid userId, string deviceIdentifier, CancellationToken cancellationToken)
        => await context.Devices.FirstOrDefaultAsync
        (
                x => x.UserId == userId
                  && x.DeviceIdentifier == deviceIdentifier,
                cancellationToken
        );
    

    public async Task AddAsync(Device device, CancellationToken cancellationToken)
        => await context.Devices.AddAsync(device, cancellationToken);
    

    public Task UpdateAsync(Device device, CancellationToken cancellationToken)
    {
        context.Devices.Update(device);
        return Task.CompletedTask;
    }

    public async Task<Device?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken = default)
        => await context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DeviceId == deviceId, cancellationToken);

    public async Task<PaginatedResult<Device>> GetAllAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Device> query = context.Devices.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.DeviceIdentifier, $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.DeviceIdentifier)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Device>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
