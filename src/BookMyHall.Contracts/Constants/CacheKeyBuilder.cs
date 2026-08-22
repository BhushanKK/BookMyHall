namespace BookMyHall.Shared.Common;

public static class CacheKeyBuilder
{
    public static string BuildPaginatedKey<T>(
        string baseKey,
        int pageNumber,
        int pageSize,
        string? searchText,
        string? sortBy,
        bool sortDescending)
    {
        return $"{baseKey}:page:" +
               $"{pageNumber}:" +
               $"{pageSize}:" +
               $"{searchText?.Trim().ToLowerInvariant() ?? string.Empty}:" +
               $"{sortBy?.Trim().ToLowerInvariant() ?? string.Empty}:" +
               $"{sortDescending}";
    }
}