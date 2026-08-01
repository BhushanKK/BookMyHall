using MediatR;
using BookMyHall.Application.Features.Master;
namespace BookMyHall.Api.Endpoints.Master;

public static class EventCategoryEndpoints
{
    public static void MapEventCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/event-categories")
            .WithTags("Event Categories")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateEventCategoryCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{eventCategoryId:guid}", async (
            Guid eventCategoryId,
            UpdateEventCategoryCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.EventCategoryId = eventCategoryId;

            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{eventCategoryId:guid}", async (
            Guid eventCategoryId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteEventCategoryCommand(eventCategoryId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapGet("/{eventCategoryId:guid}", async (
            Guid eventCategoryId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetEventCategoryByIdQuery(eventCategoryId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapPost("/search", async (
            GetEventCategoriesQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }
}