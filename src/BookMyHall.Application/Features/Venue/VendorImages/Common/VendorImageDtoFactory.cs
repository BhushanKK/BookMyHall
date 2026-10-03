using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Contracts.Venue;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

internal static class VendorImageDtoFactory
{
    private static readonly TimeSpan UrlExpiration = TimeSpan.FromMinutes(30);

    public static async Task<VendorImageDto> CreateAsync(
        VendorImage image,
        IR2StorageService storage,
        CancellationToken cancellationToken)
    {
        var imageUrl = await storage.GetPreSignedUrlAsync(
            image.ImageUrl,
            UrlExpiration,
            cancellationToken);

        string? thumbnailUrl = null;
        if (!string.IsNullOrWhiteSpace(image.ThumbnailUrl))
        {
            thumbnailUrl = await storage.GetPreSignedUrlAsync(
                image.ThumbnailUrl,
                UrlExpiration,
                cancellationToken);
        }

        return new VendorImageDto
        {
            VendorImageId = image.VendorImageId,
            VendorId = image.VendorId,
            VendorServiceId = image.VendorServiceId,
            VendorCategoryId = image.VendorService?.VendorCategoryId,
            VendorSubCategoryId = image.VendorService?.VendorSubCategoryId,
            ServiceName = image.VendorService?.ServiceName,
            VendorCategoryName = image.VendorService?.VendorSubCategory?.VendorCategory?.VendorCategoryName,
            VendorSubCategoryName = image.VendorService?.VendorSubCategory?.VendorSubCategoryName,
            ImageUrl = imageUrl,
            ThumbnailUrl = thumbnailUrl,
            DisplayOrder = image.DisplayOrder,
            IsCoverImage = image.IsCoverImage,
            IsActive = image.IsActive,
            CreatedBy = image.CreatedBy,
            CreatedDate = image.CreatedDate
        };
    }
}
