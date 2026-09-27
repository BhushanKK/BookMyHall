using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
namespace BookMyHall.Application.Abstractions.Persistence.Repositories;
public interface IVendorSubCategoryRepository
{
    Task AddAsync(VendorSubCategory vendorSubCategory,CancellationToken cancellationToken = default);
    Task UpdateAsync(VendorSubCategory vendorSubCategory,CancellationToken cancellationToken = default);
    Task<VendorSubCategory?> GetByIdAsync(Guid vendorSubCategoryId,CancellationToken cancellationToken = default);
    Task<VendorSubCategory?> GetByNameAsync( Guid vendorCategoryId,string name,CancellationToken cancellationToken = default);
    Task<VendorSubCategory?> GetByNameIncludingDeletedAsync(Guid vendorCategoryId,string name,
        CancellationToken cancellationToken = default);
    Task<PaginatedResult<VendorSubCategory>> GetAllAsync(PaginationRequest request, Guid? vendorCategoryId,
        CancellationToken cancellationToken = default);
}