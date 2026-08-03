using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class DistrictEndpoints
{
    public static void MapDistrictEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/districts")
            .WithTags("District")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateDistrictCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateDistrict")
        .WithSummary("Create District")
        .WithDescription("Creates a new district.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{districtId:guid}", async (
            Guid districtId,
            UpdateDistrictCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.DistrictId = districtId;

            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateDistrict")
        .WithSummary("Update District")
        .WithDescription("Updates an existing district.")
        .Produces<ApiResponse<DistrictDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{districtId:guid}", async (
            Guid districtId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new DeleteDistrictCommand(districtId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteDistrict")
        .WithSummary("Delete District")
        .WithDescription("Deletes an existing district.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{districtId:guid}", async (
            Guid districtId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetDistrictByIdQuery(districtId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetDistrictById")
        .WithSummary("Get District By Id")
        .WithDescription("Returns a district by its identifier.")
        .Produces<ApiResponse<DistrictDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetDistrictsQuery(request),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetDistricts")
        .WithSummary("Get Districts")
        .WithDescription("Returns a paginated list of districts.")
        .Produces<ApiResponse<PaginatedResponse<DistrictDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}