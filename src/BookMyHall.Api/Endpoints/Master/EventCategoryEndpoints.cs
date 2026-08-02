using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class EventCategoryEndpoints
{
    public static void MapEventCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/event-categories")
            .WithTags("Event Categories")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", CreateEventCategoryAsync)
            .WithName("CreateEventCategory")
            .WithSummary("Create Event Category")
            .WithDescription("Creates a new event category.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{eventCategoryId:guid}", UpdateEventCategoryAsync)
            .WithName("UpdateEventCategory")
            .WithSummary("Update Event Category")
            .WithDescription("Updates an existing event category.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{eventCategoryId:guid}", DeleteEventCategoryAsync)
            .WithName("DeleteEventCategory")
            .WithSummary("Delete Event Category")
            .WithDescription("Soft deletes an event category.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{eventCategoryId:guid}", GetEventCategoryByIdAsync)
            .WithName("GetEventCategoryById")
            .WithSummary("Get Event Category By Id")
            .WithDescription("Retrieves an event category by its unique identifier.")
            .Produces<ApiResponse<EventCategoryDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<EventCategoryDto>>(StatusCodes.Status404NotFound);

        group.MapGet("/GetAllEventCategories", GetEventCategoriesAsync)
            .WithName("GetEventCategories")
            .WithSummary("Get Event Categories")
            .WithDescription("Retrieves a paginated list of event categories.")
            .Produces<ApiResponse<PaginatedResult<EventCategoryDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaginatedResult<EventCategoryDto>>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateEventCategoryAsync(
        CreateEventCategoryCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> UpdateEventCategoryAsync(
        Guid eventCategoryId,
        UpdateEventCategoryCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        command.EventCategoryId = eventCategoryId;

        var response = await sender.Send(command, cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> DeleteEventCategoryAsync(
        Guid eventCategoryId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new DeleteEventCategoryCommand(eventCategoryId),
            cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> GetEventCategoryByIdAsync(
        Guid eventCategoryId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetEventCategoryByIdQuery(eventCategoryId),
            cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> GetEventCategoriesAsync(
        GetEventCategoriesQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return Results.Json(response, statusCode:response.StatusCode);
    }
}