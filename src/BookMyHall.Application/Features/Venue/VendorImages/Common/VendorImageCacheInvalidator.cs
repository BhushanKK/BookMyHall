using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

internal static class VendorImageCacheInvalidator
{
    public static async Task InvalidateAsync(
        ICacheService cacheService,
        Guid vendorId,
        Guid? vendorImageId,
        CancellationToken cancellationToken)
    {
        if (vendorImageId.HasValue)
        {
            await cacheService.RemoveAsync(
                VendorImageCacheKeyBuilder.BuildImageKey(vendorImageId.Value),
                cancellationToken);
        }

        await cacheService.RemoveAsync(
            VendorImageCacheKeyBuilder.BuildCoverImageKey(vendorId),
            cancellationToken);

        await cacheService.RemoveByPrefixAsync(
            VendorImageCacheKeyBuilder.BuildVendorPaginatedPrefix(vendorId),
            cancellationToken);
    }
}