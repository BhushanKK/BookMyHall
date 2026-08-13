using MediatR;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Api.Endpoints.Venue;

public static class HallEndpoints
{
    public static void MapHallEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/halls")
            .WithTags("Hall")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateHallCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("CreateHall")
        .WithSummary("Create Hall")
        .WithDescription("Creates a new hall.")
        .Produces<ApiResponse<HallDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{hallId:guid}", async (
            Guid hallId,
            UpdateHallCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.HallId = hallId;

            var response = await mediator.Send(
                command,
                cancellationToken);

            return Results.Json(
                response,
                statusCode: response.StatusCode);
        })
        .WithName("UpdateHall")
        .WithSummary("Update Hall")
        .WithDescription("Updates an existing hall.")
        .Produces<ApiResponse<HallDto>>(
            StatusCodes.Status200OK)
        .Produces(
            StatusCodes.Status400BadRequest)
        .Produces(
            StatusCodes.Status401Unauthorized)
        .Produces(
            StatusCodes.Status404NotFound)
        .Produces(
            StatusCodes.Status409Conflict);

        group.MapGet("/{hallId:guid}", async (
            Guid hallId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetHallByIdQuery(hallId),cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetHallById")
        .WithSummary("Get Hall By Id")
        .WithDescription("Returns a hall by its identifier.")
        .Produces<ApiResponse<Hall>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetHallQuery(request),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetHalls")
        .WithSummary("Get Halls")
        .WithDescription("Returns a paginated list of halls.")
        .Produces<ApiResponse<PaginatedResult<Hall>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/by-name", async (string hallName,Guid areaId,IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetHallByHallNameAndAreaQuery(hallName, areaId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetHallByHallNameAndArea")
        .WithSummary("Get Hall By Name And Area")
        .WithDescription("Returns a hall by hall name and area.")
        .Produces<ApiResponse<Hall>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);        
    }
}