namespace BookMyHall.Application.Abstractions.Caching;

public static class HallPricingCacheKeyBuilder
{
    // =========================================================
    // GET BY ID
    // =========================================================

    public static string BuildByIdKey(
        Guid hallPricingId)
    {
        return
            $"{CacheKeys.HallPricing}:{hallPricingId}";
    }

    // =========================================================
    // GET BY HALL + EVENT CATEGORY
    // =========================================================

    public static string BuildByHallAndEventCategoryKey(
        Guid hallId,
        Guid eventCategoryId)
    {
        return
            $"{CacheKeys.HallPricing}:" +
            $"hall:{hallId}:" +
            $"event-category:{eventCategoryId}";
    }

    // =========================================================
    // PAGINATED LIST
    // =========================================================

    public static string BuildPaginatedKey(
        Guid? hallId,
        int pageNumber,
        int pageSize,
        string? searchText,
        string? sortBy,
        bool sortDescending)
    {
        return
            $"{CacheKeys.HallPricingsPaged}:" +
            $"hall:{Normalize(hallId)}:" +
            $"page:{pageNumber}:" +
            $"size:{pageSize}:" +
            $"search:{Normalize(searchText)}:" +
            $"sort:{Normalize(sortBy)}:" +
            $"desc:{sortDescending}";
    }

    // =========================================================
    // PAGINATED CACHE PREFIX
    // =========================================================

    public static string BuildPaginatedPrefix()
    {
        return $"{CacheKeys.HallPricingsPaged}:";
    }

    // =========================================================
    // GUID NORMALIZATION
    // =========================================================

    private static string Normalize(Guid? value)
    {
        return value?.ToString() ?? "none";
    }

    // =========================================================
    // STRING NORMALIZATION
    // =========================================================

    private static string Normalize(string? value)
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