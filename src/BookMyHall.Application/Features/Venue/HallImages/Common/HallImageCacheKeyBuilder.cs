namespace BookMyHall.Application.Abstractions.Caching;

public static class HallImageCacheKeyBuilder
{
    public static string BuildPaginatedKey(
        Guid hallId,
        int pageNumber,
        int pageSize,
        string? searchText,
        string? sortBy,
        bool sortDescending)
    {
        return
            $"hall-images:hall:{hallId}" +
            $":page:{pageNumber}" +
            $":size:{pageSize}" +
            $":search:{Normalize(searchText)}" +
            $":sort:{Normalize(sortBy)}" +
            $":desc:{sortDescending}";
    }

    private static string Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "none"
            : value
                .Trim()
                .ToLowerInvariant()
                .Replace(":", "_");
    }
}