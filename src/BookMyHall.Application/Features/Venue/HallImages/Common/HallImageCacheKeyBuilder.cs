namespace BookMyHall.Application.Abstractions.Caching;

public static class HallImageCacheKeyBuilder
{
    // =========================================================
    // Individual Hall Image
    // =========================================================

    public static string BuildImageKey(
        Guid hallImageId)
    {
        return
            $"{CacheKeys.HallImage}:{hallImageId}";
    }


    // =========================================================
    // Hall Cover Image
    // =========================================================

    public static string BuildCoverImageKey(
        Guid hallId)
    {
        return
            $"{CacheKeys.HallCoverImage}:{hallId}";
    }


    // =========================================================
    // Paginated Hall Images
    // =========================================================
    //
    // Example:
    //
    // hallimages:page:
    //     {hallId}
    //     :page:1
    //     :size:10
    //     :search:none
    //     :sort:none
    //     :desc:true
    //
    // This intentionally starts with:
    //
    //     CacheKeys.HallImagesPaged
    //
    // so RemoveByPrefixAsync() works correctly.
    // =========================================================

    public static string BuildPaginatedKey(
        Guid hallId,
        int pageNumber,
        int pageSize,
        string? searchText,
        string? sortBy,
        bool sortDescending)
    {
        return
            $"{CacheKeys.HallImagesPaged}" +
            $"{hallId}" +
            $":page:{pageNumber}" +
            $":size:{pageSize}" +
            $":search:{Normalize(searchText)}" +
            $":sort:{Normalize(sortBy)}" +
            $":desc:{sortDescending}";
    }


    // =========================================================
    // Hall-specific Paginated Prefix
    // =========================================================
    //
    // This allows us to invalidate only one hall's image cache.
    //
    // Example:
    //
    // hallimages:page:{hallId}
    //
    // =========================================================

    public static string BuildHallPaginatedPrefix(
        Guid hallId)
    {
        return
            $"{CacheKeys.HallImagesPaged}" +
            $"{hallId}:";
    }


    // =========================================================
    // Normalize Cache-Key Values
    // =========================================================

    private static string Normalize(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "none";
        }

        return value
            .Trim()
            .ToLowerInvariant()
            .Replace(":", "_");
    }
}