using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Common.Interfaces.Repositories.Venue;

public interface IVendorImageRepository
{
    Task<VendorImage?> GetByIdAsync(Guid vendorImageId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<VendorImage>> GetByVendorIdAsync(Guid vendorId, PaginationRequest request, CancellationToken cancellationToken = default, Guid? vendorServiceId = null, Guid? vendorCategoryId = null, Guid? vendorSubCategoryId = null);
    Task<VendorImage?> GetCoverImageAsync(Guid vendorId, CancellationToken cancellationToken = default, Guid? vendorServiceId = null);
    Task AddAsync(VendorImage vendorImage, CancellationToken cancellationToken = default);
    Task UpdateAsync(VendorImage vendorImage, CancellationToken cancellationToken = default);
    Task ClearOtherCoverImagesAsync(Guid vendorId, Guid? exceptVendorImageId, CancellationToken cancellationToken = default, Guid? vendorServiceId = null);
}
