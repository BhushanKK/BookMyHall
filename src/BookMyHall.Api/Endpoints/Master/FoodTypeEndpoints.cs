using MediatR;
using BookMyHall.Application.Features.Master;

namespace BookMyHall.Api.Endpoints.Master;

public static class FoodTypeEndpoints
{
    public static void MapFoodTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/food-types")
            .WithTags("Food Types")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateFoodTypeCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{foodTypeId:guid}", async (
            Guid foodTypeId,
            UpdateFoodTypeCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.FoodTypeId = foodTypeId;

            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{foodTypeId:guid}", async (
            Guid foodTypeId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteFoodTypeCommand(foodTypeId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapGet("/{foodTypeId:guid}", async (
            Guid foodTypeId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetFoodTypeByIdQuery(foodTypeId),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapPost("/search", async (
            GetFoodTypesQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }
}