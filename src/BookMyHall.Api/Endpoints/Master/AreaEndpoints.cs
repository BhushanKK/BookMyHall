using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class AreaEndpoints
{
    public static void MapAreaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/areas")
            .WithTags("Areas")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", CreateAreaAsync)
            .WithName("CreateArea")
            .WithSummary("Create Area")
            .WithDescription("Creates a new area.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{areaId:guid}", UpdateAreaAsync)
            .WithName("UpdateArea")
            .WithSummary("Update Area")
            .WithDescription("Updates an existing area.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{areaId:guid}", DeleteAreaAsync)
            .WithName("DeleteArea")
            .WithSummary("Delete Area")
            .WithDescription("Soft deletes an area.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{areaId:guid}", GetAreaByIdAsync)
            .WithName("GetAreaById")
            .WithSummary("Get Area By Id")
            .WithDescription("Retrieves an area by its unique identifier.")
            .Produces<ApiResponse<AreaDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<AreaDto>>(StatusCodes.Status404NotFound);

        group.MapPost("/search", GetAreasAsync)
            .WithName("GetAreas")
            .WithSummary("Get Areas")
            .WithDescription("Retrieves a paginated list of areas.")
            .Produces<ApiResponse<PaginatedResult<AreaDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaginatedResult<AreaDto>>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateAreaAsync(
        CreateAreaCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> UpdateAreaAsync(
        Guid areaId,
        UpdateAreaCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        command.AreaId = areaId;

        var response = await sender.Send(command, cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> DeleteAreaAsync(
        Guid areaId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new DeleteAreaCommand(areaId),
            cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetAreaByIdAsync(
        Guid areaId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetAreaByIdQuery(areaId),
            cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetAreasAsync(
        GetAreasQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }
}