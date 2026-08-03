using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class EventCategoryEndpoints
{
    public static void MapEventCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/event-categories")
            .WithTags("Event Category")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateEventCategoryCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateEventCategory")
        .WithSummary("Create Event Category")
        .WithDescription("Creates a new event category.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{eventCategoryId:guid}", async (
            Guid eventCategoryId,
            UpdateEventCategoryCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.EventCategoryId = eventCategoryId;

            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateEventCategory")
        .WithSummary("Update Event Category")
        .WithDescription("Updates an existing event category.")
        .Produces<ApiResponse<EventCategoryDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{eventCategoryId:guid}", async (
            Guid eventCategoryId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new DeleteEventCategoryCommand(eventCategoryId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteEventCategory")
        .WithSummary("Delete Event Category")
        .WithDescription("Deletes an existing event category.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{eventCategoryId:guid}", async (
            Guid eventCategoryId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetEventCategoryByIdQuery(eventCategoryId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetEventCategoryById")
        .WithSummary("Get Event Category By Id")
        .WithDescription("Returns an event category by its identifier.")
        .Produces<ApiResponse<EventCategoryDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetEventCategoriesQuery(request),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetEventCategories")
        .WithSummary("Get Event Categories")
        .WithDescription("Returns a paginated list of event categories.")
        .Produces<ApiResponse<PaginatedResponse<EventCategoryDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}