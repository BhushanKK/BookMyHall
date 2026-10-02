using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public static class VendorImageCacheKeyBuilder
{
    public static string BuildImageKey(Guid vendorImageId)
        => $"{CacheKeys.VendorImage}:{vendorImageId}";

    public static string BuildCoverImageKey(Guid vendorId)
        => $"{CacheKeys.VendorCoverImage}:{vendorId}";

    public static string BuildPaginatedKey(
        Guid vendorId,
        int pageNumber,
        int pageSize,
        string? searchText,
        string? sortBy,
        bool sortDescending)
        => $"{CacheKeys.VendorImagesPaged}:{vendorId}:page:{pageNumber}:size:{pageSize}:search:{Normalize(searchText)}:sort:{Normalize(sortBy)}:desc:{sortDescending}";

    public static string BuildVendorPaginatedPrefix(Guid vendorId)
        => $"{CacheKeys.VendorImagesPaged}:{vendorId}:";

    private static string Normalize(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? "none"
            : value.Trim().ToLowerInvariant().Replace(":", "_");
}