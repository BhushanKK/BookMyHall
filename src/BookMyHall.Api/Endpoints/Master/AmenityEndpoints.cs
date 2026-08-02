using MediatR;

using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public static class AmenityEndpoints
{
    public static void MapAmenityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/amenities")
            .WithTags("Amenities")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", CreateAmenityAsync)
            .WithName("CreateAmenity")
            .WithSummary("Create Amenity")
            .WithDescription("Creates a new amenity.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{amenityId:guid}", UpdateAmenityAsync)
            .WithName("UpdateAmenity")
            .WithSummary("Update Amenity")
            .WithDescription("Updates an existing amenity.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{amenityId:guid}", DeleteAmenityAsync)
            .WithName("DeleteAmenity")
            .WithSummary("Delete Amenity")
            .WithDescription("Soft deletes an amenity.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{amenityId:guid}", GetAmenityByIdAsync)
            .WithName("GetAmenityById")
            .WithSummary("Get Amenity By Id")
            .WithDescription("Retrieves an amenity by its unique identifier.")
            .Produces<ApiResponse<AmenityDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<AmenityDto>>(StatusCodes.Status404NotFound);

        group.MapPost("/search", GetAmenitiesAsync)
            .WithName("GetAmenities")
            .WithSummary("Get Amenities")
            .WithDescription("Retrieves a paginated list of amenities.")
            .Produces<ApiResponse<PaginatedResult<AmenityDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaginatedResult<AmenityDto>>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateAmenityAsync(
        CreateAmenityCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> UpdateAmenityAsync(
        Guid amenityId,
        UpdateAmenityCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        command.AmenityId = amenityId;
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> DeleteAmenityAsync(
        Guid amenityId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new DeleteAmenityCommand(amenityId),
            cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetAmenityByIdAsync(
        Guid amenityId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetAmenityByIdQuery(amenityId),
            cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetAmenitiesAsync(
        GetAmenitiesQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }
}