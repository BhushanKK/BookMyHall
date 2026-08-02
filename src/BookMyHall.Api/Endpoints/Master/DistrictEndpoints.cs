using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class DistrictEndpoints
{
    public static void MapDistrictEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/districts")
            .WithTags("Districts")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", CreateDistrictAsync)
            .WithName("CreateDistrict")
            .WithSummary("Create District")
            .WithDescription("Creates a new district.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{districtId:guid}", UpdateDistrictAsync)
            .WithName("UpdateDistrict")
            .WithSummary("Update District")
            .WithDescription("Updates an existing district.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{districtId:guid}", DeleteDistrictAsync)
            .WithName("DeleteDistrict")
            .WithSummary("Delete District")
            .WithDescription("Soft deletes a district.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{districtId:guid}", GetDistrictByIdAsync)
            .WithName("GetDistrictById")
            .WithSummary("Get District By Id")
            .WithDescription("Retrieves a district by its unique identifier.")
            .Produces<ApiResponse<DistrictDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<DistrictDto>>(StatusCodes.Status404NotFound);

        group.MapPost("/search", GetDistrictsAsync)
            .WithName("GetDistricts")
            .WithSummary("Get Districts")
            .WithDescription("Retrieves a paginated list of districts.")
            .Produces<ApiResponse<PaginatedResult<DistrictDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaginatedResult<DistrictDto>>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateDistrictAsync(
        CreateDistrictCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> UpdateDistrictAsync(
        Guid districtId,
        UpdateDistrictCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        command.DistrictId = districtId;

        var response = await sender.Send(command, cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> DeleteDistrictAsync(
        Guid districtId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new DeleteDistrictCommand(districtId),
            cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetDistrictByIdAsync(
        Guid districtId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetDistrictByIdQuery(districtId),
            cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetDistrictsAsync(
        GetDistrictsQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }
}