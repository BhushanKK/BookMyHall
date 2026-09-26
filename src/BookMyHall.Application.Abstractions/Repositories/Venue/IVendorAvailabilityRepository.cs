using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IVendorAvailabilityRepository
{
    Task<VendorAvailability?> GetByIdAsync(Guid vendorAvailabilityId,CancellationToken cancellationToken);
    Task<PaginatedResult<VendorAvailability>> GetAllAsync(PaginationRequest request,Guid? vendorId,CancellationToken cancellationToken);
    Task<bool> ExistsOverlappingAsync(Guid vendorId,short? dayOfWeek,DateOnly? availableDate,TimeOnly? startTime,
        TimeOnly? endTime,Guid? excludeVendorAvailabilityId,CancellationToken cancellationToken);
    Task AddAsync(VendorAvailability vendorAvailability,CancellationToken cancellationToken);
    Task UpdateAsync(VendorAvailability vendorAvailability,CancellationToken cancellationToken);
}