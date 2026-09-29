using BookMyHall.Domain.Venue;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;
public interface IVendorPackageRepository
{
    Task AddAsync(VendorPackage vendorPackage, CancellationToken cancellationToken = default);
    Task UpdateAsync(VendorPackage vendorPackage, CancellationToken cancellationToken = default);
    Task<VendorPackage?> GetByIdAsync(Guid vendorPackageId, CancellationToken cancellationToken = default);
    Task<VendorPackage?> GetByNameIncludingDeletedAsync(string packageName, Guid vendorId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, int limit = 20, CancellationToken cancellationToken = default);
    Task<PaginatedResult<VendorPackage>> GetAllAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken);
}
