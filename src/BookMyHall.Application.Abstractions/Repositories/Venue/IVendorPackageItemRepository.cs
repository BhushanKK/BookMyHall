using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IVendorPackageItemRepository
{
    Task AddAsync(VendorPackageItem vendorPackageItem,CancellationToken cancellationToken = default);
    Task UpdateAsync(VendorPackageItem vendorPackageItem,CancellationToken cancellationToken = default);
    Task<VendorPackageItem?> GetByIdAsync(Guid vendorPackageItemId,CancellationToken cancellationToken = default);
     Task<VendorPackageItem?> GetByPackageAndServiceIncludingDeletedAsync(Guid vendorPackageId,Guid vendorServiceId,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid vendorPackageId,Guid vendorServiceId,Guid? excludeVendorPackageItemId,
        CancellationToken cancellationToken = default);
    Task<PaginatedResult<VendorPackageItem>> GetAllAsync(PaginationRequest request,Guid? vendorPackageId,
        CancellationToken cancellationToken = default);
}