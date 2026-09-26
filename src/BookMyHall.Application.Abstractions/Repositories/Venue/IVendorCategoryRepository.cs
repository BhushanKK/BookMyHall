using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IVendorCategoryRepository
{
    Task AddAsync(VendorCategory vendor,CancellationToken cancellationToken = default);
    Task UpdateAsync(VendorCategory vendor,CancellationToken cancellationToken = default);
    Task<VendorCategory?> GetByIdAsync( Guid vendorId,CancellationToken cancellationToken = default);
    Task<VendorCategory?> GetByNameAsync(string businessName,CancellationToken cancellationToken = default);
    Task<VendorCategory?> GetByNameIncludingDeletedAsync(string businessName,CancellationToken cancellationToken = default);
    Task<PaginatedResult<VendorCategory>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
}
