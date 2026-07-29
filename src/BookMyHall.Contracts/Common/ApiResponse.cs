namespace BookMyHall.Contracts.Common;

public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyCollection<string> Errors { get; init; } = [];
    public Guid? id { get; init; }
    public static ApiResponse<T> Success(T data, string message, Guid? id = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            id=id
        };
    }
    public static ApiResponse<T> Failure(string message, params string[] errors)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors
        };
    }
}