using MediatR;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Identity;

public static class UserPreferenceEndpoints
{
    public static void MapUserPreferenceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/user-preferences")
            .WithTags("User Preferences")
            .RequireAuthorization();

        group.MapGet("/", GetUserPreferenceAsync)
            .WithName("GetUserPreference")
            .WithSummary("Get User Preference")
            .WithDescription("Retrieves the preferences of the authenticated user.")
            .Produces<ApiResponse<UserPreferenceDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<UserPreferenceDto>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<UserPreferenceDto>>(StatusCodes.Status404NotFound);

        group.MapPut("/", UpdateUserPreferenceAsync)
            .WithName("UpdateUserPreference")
            .WithSummary("Update User Preference")
            .WithDescription("Updates the preferences of the authenticated user.")
            .Produces<ApiResponse<UserPreferenceDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<UserPreferenceDto>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<UserPreferenceDto>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<UserPreferenceDto>>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetUserPreferenceAsync(Guid userId,ISender sender,CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetUserPreferenceQuery(userId),cancellationToken);
        return Results.Json(response,statusCode: response.StatusCode);
    }

    private static async Task<IResult> UpdateUserPreferenceAsync(Guid userId,
        UpdateUserPreferenceCommand command,
        ISender sender,CancellationToken cancellationToken)
    {
        command.UserId = userId;
        var response = await sender.Send(command,cancellationToken);
        return Results.Json(response,statusCode: response.StatusCode);
    }
}