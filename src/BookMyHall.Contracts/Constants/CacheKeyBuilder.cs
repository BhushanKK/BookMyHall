namespace BookMyHall.Contracts.Constants;
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
        return
            $"{cacheKey}:" +
            $"page:{pageNumber}:" +
            $"size:{pageSize}:" +
            $"search:{searchText}:" +
            $"sort:{sortBy}:" +
            $"desc:{sortDescending}";
    }
}