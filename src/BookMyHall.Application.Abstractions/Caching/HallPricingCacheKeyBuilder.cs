namespace BookMyHall.Application.Abstractions.Caching;

public static class HallPricingCacheKeyBuilder
{
    // =========================================================
    // GET BY ID
    // =========================================================

    public static string BuildByIdKey(Guid hallPricingId)
        => $"{CacheKeys.HallPricing}:{hallPricingId}";
    

    // =========================================================
    // GET BY HALL + EVENT CATEGORY
    // =========================================================

    public static string BuildByHallAndEventCategoryKey(Guid hallId, Guid eventCategoryId)
        => $"{CacheKeys.HallPricing}: hall:{hallId}: event-category:{eventCategoryId}";

    // =========================================================
    // PAGINATED LIST
    // =========================================================

    public static string BuildPaginatedKey(
        int pageNumber,
        int pageSize,
        string? searchText,
        string? sortBy,
        bool sortDescending)
    {
        return
            $"{CacheKeys.HallPricingsPaged}:" +
            $"page:{pageNumber}:" +
            $"size:{pageSize}:" +
            $"search:{Normalize(searchText)}:" +
            $"sort:{Normalize(sortBy)}:" +
            $"desc:{sortDescending}";
    }

    // =========================================================
    // ALL PAGINATED CACHE KEYS
    // =========================================================

    public static string BuildPaginatedPrefix()
        => $"{CacheKeys.HallPricingsPaged}:";

    // =========================================================
    // NORMALIZE CACHE KEY VALUES
    // =========================================================

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "none";

        return value
            .Trim()
            .ToLowerInvariant()
            .Replace(":", "_");
    }
}