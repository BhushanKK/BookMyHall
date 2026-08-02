using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Api.Endpoints.Master;
public static class StateEndpoints
{
    public static void MapStateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/states")
            .WithTags("States")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", CreateStateAsync)
            .WithName("CreateState")
            .WithSummary("Create State")
            .WithDescription("Creates a new state.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{stateId:guid}", UpdateStateAsync)
            .WithName("UpdateState")
            .WithSummary("Update State")
            .WithDescription("Updates an existing state.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{stateId:guid}", DeleteStateAsync)
            .WithName("DeleteState")
            .WithSummary("Delete State")
            .WithDescription("Soft deletes a state.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{stateId:guid}", GetStateByIdAsync)
            .WithName("GetStateById")
            .WithSummary("Get State By Id")
            .WithDescription("Retrieves a state by its unique identifier.")
            .Produces<ApiResponse<StateDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<StateDto>>(StatusCodes.Status404NotFound);

        group.MapPost("/search", GetStatesAsync)
            .WithName("GetStates")
            .WithSummary("Get States")
            .WithDescription("Retrieves a paginated list of states.")
            .Produces<ApiResponse<PaginatedResult<StateDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaginatedResult<StateDto>>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateStateAsync(CreateStateCommand command,ISender sender,CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> UpdateStateAsync(Guid stateId,UpdateStateCommand command,ISender sender,CancellationToken cancellationToken)
    {
        command.StateId = stateId;
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> DeleteStateAsync(Guid stateId,ISender sender,CancellationToken cancellationToken)
    {
        var response = await sender.Send(new DeleteStateCommand(stateId),cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetStateByIdAsync(Guid stateId,ISender sender,CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetStateByIdQuery(stateId), cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetStatesAsync(GetStateByIdQuery query,ISender sender,CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }
}