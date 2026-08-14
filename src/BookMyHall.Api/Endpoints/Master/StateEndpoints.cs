using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Api.Endpoints.Master;

public static class StateEndpoints
{
    public static void MapStateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/states")
            .WithTags("State")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateStateCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateState")
        .WithSummary("Create State")
        .WithDescription("Creates a new state.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{stateId:guid}", async (
            Guid stateId,
            UpdateStateCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.StateId = stateId;

            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateState")
        .WithSummary("Update State")
        .WithDescription("Updates an existing state.")
        .Produces<ApiResponse<StateDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{stateId:guid}", async (
            Guid stateId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new DeleteStateCommand(stateId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteState")
        .WithSummary("Delete State")
        .WithDescription("Deletes an existing state.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{stateId:guid}", async (
            Guid stateId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetStateByIdQuery(stateId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetStateById")
        .WithSummary("Get State By Id")
        .WithDescription("Returns a state by its identifier.")
        .Produces<ApiResponse<StateDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetStateQuery(request),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetStates")
        .WithSummary("Get States")
        .WithDescription("Returns a paginated list of states.")
        .Produces<ApiResponse<PaginatedResponse<StateDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/by-name/{stateName}", async (
            string stateName,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetStateByStateNameQuery(stateName),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetStateByStateName")
        .WithSummary("Get State By State Name")
        .WithDescription("Returns a state by its name.")
        .Produces<ApiResponse<StateDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/by-code/{stateCode}", async (
            string stateCode,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetStateByStateCodeQuery(stateCode),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetStateByStateCode")
        .WithSummary("Get State By State Code")
        .WithDescription("Returns a state by its code.")
        .Produces<ApiResponse<StateDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
    }
}