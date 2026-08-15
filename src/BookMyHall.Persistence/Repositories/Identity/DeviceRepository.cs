using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Domain.Identity;
using BookMyHall.Persistence.Context;

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
}