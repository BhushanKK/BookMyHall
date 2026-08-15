using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IDeviceRepository
{
    Task<Device?> GetByDeviceIdentifierAsync(Guid userId, string deviceIdentifier, CancellationToken cancellationToken);
    Task AddAsync(Device device, CancellationToken cancellationToken);
    Task UpdateAsync(Device device, CancellationToken cancellationToken);
    Task<PaginatedResult<Device>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken= default);
    Task<Device?> GetByIdAsync(Guid deviceId,CancellationToken cancellationToken=default);
}