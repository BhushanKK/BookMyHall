using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class FoodTypeEndpoints
{
    public static void MapFoodTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/food-types")
            .WithTags("Food Types")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", CreateFoodTypeAsync)
            .WithName("CreateFoodType")
            .WithSummary("Create Food Type")
            .WithDescription("Creates a new food type.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{foodTypeId:guid}", UpdateFoodTypeAsync)
            .WithName("UpdateFoodType")
            .WithSummary("Update Food Type")
            .WithDescription("Updates an existing food type.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{foodTypeId:guid}", DeleteFoodTypeAsync)
            .WithName("DeleteFoodType")
            .WithSummary("Delete Food Type")
            .WithDescription("Soft deletes a food type.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{foodTypeId:guid}", GetFoodTypeByIdAsync)
            .WithName("GetFoodTypeById")
            .WithSummary("Get Food Type By Id")
            .WithDescription("Retrieves a food type by its unique identifier.")
            .Produces<ApiResponse<FoodTypeDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<FoodTypeDto>>(StatusCodes.Status404NotFound);

        group.MapPost("/search", GetFoodTypesAsync)
            .WithName("GetFoodTypes")
            .WithSummary("Get Food Types")
            .WithDescription("Retrieves a paginated list of food types.")
            .Produces<ApiResponse<PaginatedResult<FoodTypeDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaginatedResult<FoodTypeDto>>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateFoodTypeAsync(
        CreateFoodTypeCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> UpdateFoodTypeAsync(
        Guid foodTypeId,
        UpdateFoodTypeCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        command.FoodTypeId = foodTypeId;

        var response = await sender.Send(command, cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> DeleteFoodTypeAsync(
        Guid foodTypeId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new DeleteFoodTypeCommand(foodTypeId),
            cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetFoodTypeByIdAsync(
        Guid foodTypeId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetFoodTypeByIdQuery(foodTypeId),
            cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetFoodTypesAsync(
        GetFoodTypesQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }
}