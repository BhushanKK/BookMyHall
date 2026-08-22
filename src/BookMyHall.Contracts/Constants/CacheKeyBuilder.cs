namespace BookMyHall.Application.Abstractions.Caching;

public static class CacheKeyBuilder
{
    public static string BuildPaginatedKey<T>(
        string prefix,
        int pageNumber,
        int pageSize,
        string? searchText,
        string? sortBy,
        bool sortDescending)
    {
        return $"{prefix}:page:{pageNumber}:{pageSize}:{searchText?.Trim().ToLowerInvariant()}:{sortBy?.Trim().ToLowerInvariant()}:{sortDescending}";
    }
}