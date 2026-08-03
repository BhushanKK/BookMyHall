// using MediatR;

// using BookMyHall.Contracts.Common;

// namespace BookMyHall.Application.Features.Master;

// public static class AmenityEndpoints
// {
//     public static void MapAmenityEndpoints(this IEndpointRouteBuilder app)
//     {
//         var group = app.MapGroup("/api/amenities")
//             .WithTags("Amenities")
//             .RequireAuthorization(policy => policy.RequireRole("Admin"));

//         group.MapPost("/", CreateAmenityAsync)
//             .WithName("CreateAmenity")
//             .WithSummary("Create Amenity")
//             .WithDescription("Creates a new amenity.")
//             .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
//             .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
//             .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

//         group.MapPut("/{amenityId:guid}", UpdateAmenityAsync)
//             .WithName("UpdateAmenity")
//             .WithSummary("Update Amenity")
//             .WithDescription("Updates an existing amenity.")
//             .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
//             .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
//             .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
//             .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

//         group.MapDelete("/{amenityId:guid}", DeleteAmenityAsync)
//             .WithName("DeleteAmenity")
//             .WithSummary("Delete Amenity")
//             .WithDescription("Soft deletes an amenity.")
//             .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
//             .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

//         group.MapGet("/{amenityId:guid}", GetAmenityByIdAsync)
//             .WithName("GetAmenityById")
//             .WithSummary("Get Amenity By Id")
//             .WithDescription("Retrieves an amenity by its unique identifier.")
//             .Produces<ApiResponse<AmenityDto>>(StatusCodes.Status200OK)
//             .Produces<ApiResponse<AmenityDto>>(StatusCodes.Status404NotFound);

//         group.MapGet("/GetAllAmenities", GetAmenitiesAsync)
//             .WithName("GetAmenities")
//             .WithSummary("Get Amenities")
//             .WithDescription("Retrieves a paginated list of amenities.")
//             .Produces<ApiResponse<PaginatedResult<AmenityDto>>>(StatusCodes.Status200OK)
//             .Produces<ApiResponse<PaginatedResult<AmenityDto>>>(StatusCodes.Status400BadRequest);
//     }

//     private static async Task<IResult> CreateAmenityAsync(
//         CreateAmenityCommand command,
//         ISender sender,
//         CancellationToken cancellationToken)
//     {
//         var response = await sender.Send(command, cancellationToken);
//         return Results.Json(response, statusCode:response.StatusCode);
//     }

//     private static async Task<IResult> UpdateAmenityAsync(
//         Guid amenityId,
//         UpdateAmenityCommand command,
//         ISender sender,
//         CancellationToken cancellationToken)
//     {
//         command.AmenityId = amenityId;
//         var response = await sender.Send(command, cancellationToken);
//         return Results.Json(response, statusCode: response.StatusCode);
//     }

//     private static async Task<IResult> DeleteAmenityAsync(
//         Guid amenityId,
//         ISender sender,
//         CancellationToken cancellationToken)
//     {
//         var response = await sender.Send(
//             new DeleteAmenityCommand(amenityId),
//             cancellationToken);

//         return Results.Json(response, statusCode:response.StatusCode);
//     }

//     private static async Task<IResult> GetAmenityByIdAsync(
//         Guid amenityId,
//         ISender sender,
//         CancellationToken cancellationToken)
//     {
//         var response = await sender.Send(
//             new GetAmenityByIdQuery(amenityId),
//             cancellationToken);

//         return Results.Json(response, statusCode:response.StatusCode);
//     }

//     private static async Task<IResult> GetAmenitiesAsync(
//         [AsParameters] PaginationRequest request,
//         ISender sender,
//         CancellationToken cancellationToken)
//     {
//         var response = await sender.Send(new GetAmenitiesQuery(request), cancellationToken);
//         return Results.Json(response, statusCode:response.StatusCode);
//     }
// }
using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class AmenityEndpoints
{
    public static void MapAmenityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/amenities")
            .WithTags("Amenities")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateAmenityCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateAmenity")
        .WithSummary("Create Amenity")
        .WithDescription("Creates a new amenity.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{amenityId:guid}", async (
            Guid amenityId,
            UpdateAmenityCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.AmenityId = amenityId;

            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateAmenity")
        .WithSummary("Update Amenity")
        .WithDescription("Updates an existing amenity.")
        .Produces<ApiResponse<AmenityDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{amenityId:guid}", async (
            Guid amenityId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new DeleteAmenityCommand(amenityId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteAmenity")
        .WithSummary("Delete Amenity")
        .WithDescription("Deletes an existing amenity.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{amenityId:guid}", async (
            Guid amenityId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetAmenityByIdQuery(amenityId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetAmenityById")
        .WithSummary("Get Amenity By Id")
        .WithDescription("Returns an amenity by its identifier.")
        .Produces<ApiResponse<AmenityDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetAmenitiesQuery(request),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetAmenities")
        .WithSummary("Get Amenities")
        .WithDescription("Returns a paginated list of amenities.")
        .Produces<ApiResponse<PaginatedResult<AmenityDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}