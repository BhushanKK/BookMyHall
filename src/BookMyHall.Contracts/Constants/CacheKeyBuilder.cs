namespace BookMyHall.Contracts.Constants;

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
        return
            $"{prefix}:" +
            $"page:{pageNumber}:" +
            $"size:{pageSize}:" +
            $"search:{Normalize(searchText)}:" +
            $"sort:{Normalize(sortBy)}:" +
            $"desc:{sortDescending}";
    }

    private static string Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToLowerInvariant();
    }
}