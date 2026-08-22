public static class CacheKeyBuilder
{
    public static string BuildPaginatedKey<T>(
        string cacheKey,
        int pageNumber,
        int pageSize,
        string? searchText,
        string? sortBy,
        bool sortDescending)
    {
        return $"{cacheKey}:page:{pageNumber}" +
               $":size:{pageSize}" +
               $":search:{searchText?.Trim().ToLowerInvariant() ?? string.Empty}" +
               $":sort:{sortBy?.Trim().ToLowerInvariant() ?? string.Empty}" +
               $":desc:{sortDescending}";
    }
}