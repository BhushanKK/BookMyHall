using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IVendorServiceRepository
{
    Task AddAsync(VendorService vendorService, CancellationToken cancellationToken = default);
    Task UpdateAsync(VendorService vendorService, CancellationToken cancellationToken = default);
    Task<VendorService?> GetByIdAsync(Guid vendorServiceId, CancellationToken cancellationToken = default);
    Task<VendorService?> GetByNameAsync(Guid vendorId, string serviceName, CancellationToken cancellationToken = default);
    Task<VendorService?> GetByNameIncludingDeletedAsync(Guid vendorId, string serviceName, CancellationToken cancellationToken = default);
    Task<PaginatedResult<VendorService>> GetAllAsync(PaginationRequest request, Guid? vendorId, Guid? vendorCategoryId,Guid? vendorSubCategoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, int limit = 20, CancellationToken cancellationToken = default);
}