using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class FoodTypeEndpoints
{
    public static void MapFoodTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/food-types")
            .WithTags("Food Type")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateFoodTypeCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateFoodType")
        .WithSummary("Create Food Type")
        .WithDescription("Creates a new food type.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{foodTypeId:guid}", async (
            Guid foodTypeId,
            UpdateFoodTypeCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.FoodTypeId = foodTypeId;

            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateFoodType")
        .WithSummary("Update Food Type")
        .WithDescription("Updates an existing food type.")
        .Produces<ApiResponse<FoodTypeDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{foodTypeId:guid}", async (
            Guid foodTypeId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new DeleteFoodTypeCommand(foodTypeId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteFoodType")
        .WithSummary("Delete Food Type")
        .WithDescription("Deletes an existing food type.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{foodTypeId:guid}", async (
            Guid foodTypeId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetFoodTypeByIdQuery(foodTypeId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetFoodTypeById")
        .WithSummary("Get Food Type By Id")
        .WithDescription("Returns a food type by its identifier.")
        .Produces<ApiResponse<FoodTypeDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetFoodTypesQuery(request),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetFoodTypes")
        .WithSummary("Get Food Types")
        .WithDescription("Returns a paginated list of food types.")
        .Produces<ApiResponse<PaginatedResponse<FoodTypeDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}