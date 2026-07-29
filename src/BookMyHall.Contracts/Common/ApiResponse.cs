using System.Net;

namespace BookMyHall.Contracts.Common;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public int StatusCode { get; init; }

    public T? Data { get; init; }

    public static ApiResponse<T> SuccessResponse(
        T? data,
        string message = "Success",
        HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = (int)statusCode
        };

    public static ApiResponse<T> FailureResponse(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        T? data = default)
        => new()
        {
            Success = false,
            Data = data,
            Message = message,
            StatusCode = (int)statusCode
        };
}