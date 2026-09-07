namespace BookMyHall.Application.Abstractions.Caching;

public static class HallBlockCacheKeyBuilder
{
    // =========================================================
    // Individual Hall Block
    // =========================================================

    public static string BuildByIdKey(
        Guid hallBlockId)
        => $"{CacheKeys.HallBlock}:{hallBlockId}";


    // =========================================================
    // Paginated Hall Blocks
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
            $"{CacheKeys.HallBlocksPaged}" +
            $"hall:{Normalize(hallId)}" +
            $":page:{pageNumber}" +
            $":size:{pageSize}" +
            $":search:{Normalize(searchText)}" +
            $":sort:{Normalize(sortBy)}" +
            $":desc:{sortDescending}";
    }


    // =========================================================
    // Paginated Cache Prefix
    // =========================================================

    public static string BuildPaginatedPrefix()
        => CacheKeys.HallBlocksPaged;


    // =========================================================
    // Normalize Guid
    // =========================================================

    private static string Normalize(
        Guid? value)
        => value?.ToString() ?? "none";


    // =========================================================
    // Normalize String
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