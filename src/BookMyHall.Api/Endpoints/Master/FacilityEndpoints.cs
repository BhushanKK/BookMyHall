using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class FacilityEndpoints
{
    public static void MapFacilityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/facilities")
            .WithTags("Facilities")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", CreateFacilityAsync)
            .WithName("CreateFacility")
            .WithSummary("Create Facility")
            .WithDescription("Creates a new facility.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{facilityId:guid}", UpdateFacilityAsync)
            .WithName("UpdateFacility")
            .WithSummary("Update Facility")
            .WithDescription("Updates an existing facility.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{facilityId:guid}", DeleteFacilityAsync)
            .WithName("DeleteFacility")
            .WithSummary("Delete Facility")
            .WithDescription("Soft deletes a facility.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{facilityId:guid}", GetFacilityByIdAsync)
            .WithName("GetFacilityById")
            .WithSummary("Get Facility By Id")
            .WithDescription("Retrieves a facility by its unique identifier.")
            .Produces<ApiResponse<FacilityDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<FacilityDto>>(StatusCodes.Status404NotFound);

        group.MapGet("/GetAllFacilities", GetFacilitiesAsync)
            .WithName("GetFacilities")
            .WithSummary("Get Facilities")
            .WithDescription("Retrieves a paginated list of facilities.")
            .Produces<ApiResponse<PaginatedResult<FacilityDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaginatedResult<FacilityDto>>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateFacilityAsync(
        CreateFacilityCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> UpdateFacilityAsync(
        Guid facilityId,
        UpdateFacilityCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        command.FacilityId = facilityId;

        var response = await sender.Send(command, cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> DeleteFacilityAsync(
        Guid facilityId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new DeleteFacilityCommand(facilityId),
            cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> GetFacilityByIdAsync(
        Guid facilityId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetFacilityByIdQuery(facilityId),
            cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> GetFacilitiesAsync(
        GetFacilitiesQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }
}