namespace BookMyHall.Contracts.Common;

public sealed class PaginationRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
}

public sealed class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}