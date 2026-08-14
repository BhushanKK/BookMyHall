using MediatR;

using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Venue;

public static class HallBlockEndpoints
{
    public static void MapHallBlockEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/hall-blocks")
            .WithTags("Hall Block")
            .RequireAuthorization(policy =>policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateHallBlockCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("CreateHallBlock")
        .WithSummary("Create Hall Block")
        .WithDescription("Creates a new hall block.")
        .Produces<ApiResponse<HallBlockDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{hallBlockId:guid}", async (
            Guid hallBlockId,UpdateHallBlockCommand command,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            command.HallBlockId = hallBlockId;
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("UpdateHallBlock")
        .WithSummary("Update Hall Block")
        .WithDescription("Updates an existing hall block.")
        .Produces<ApiResponse<HallBlockDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{hallBlockId:guid}", async (Guid hallBlockId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeleteHallBlockCommand(hallBlockId),cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("DeleteHallBlock")
        .WithSummary("Delete Hall Block")
        .WithDescription("Deletes an existing hall block.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{hallBlockId:guid}", async (Guid hallBlockId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetHallBlockByIdQuery(hallBlockId),cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetHallBlockById")
        .WithSummary("Get Hall Block By Id")
        .WithDescription("Returns a hall block by its identifier.")
        .Produces<ApiResponse<HallBlockDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetHallBlocksQuery(request),cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetHallBlocks")
        .WithSummary("Get Hall Blocks")
        .WithDescription("Returns a paginated list of hall blocks.")
        .Produces<ApiResponse<PaginatedResponse<HallBlockDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}