using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IVendorServiceAreaRepository
{
    Task AddAsync(VendorServiceArea vendorServiceArea, CancellationToken cancellationToken = default);
    Task UpdateAsync(VendorServiceArea vendorServiceArea, CancellationToken cancellationToken = default);
    Task<VendorServiceArea?> GetByIdAsync(Guid vendorServiceAreaId, CancellationToken cancellationToken = default);

    Task<VendorServiceArea?> GetByLocationIncludingDeletedAsync(Guid vendorId, Guid? stateId, Guid? cityId,
       Guid? areaId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid vendorId, Guid? stateId, Guid? cityId, Guid? areaId, Guid? excludeVendorServiceAreaId,
        CancellationToken cancellationToken = default);
    Task<PaginatedResult<VendorServiceArea>> GetAllAsync(PaginationRequest request, Guid? vendorId, CancellationToken cancellationToken = default);
}