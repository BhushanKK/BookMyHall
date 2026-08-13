using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Api.Endpoints.Venue;
public static class HallPricingEndpoints
{
    public static void MapHallPricingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/hall-pricings")
            .WithTags("Hall Pricing")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateHallPricingCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateHallPricing")
        .WithSummary("Create Hall Pricing")
        .WithDescription("Creates a new pricing configuration for a hall.")
        .Produces<ApiResponse<HallPricingDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);


        // Update Hall Pricing
        group.MapPut("/{hallPricingId:guid}", async (
            Guid hallPricingId,
            UpdateHallPricingCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.HallPricingId = hallPricingId;
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateHallPricing")
        .WithSummary("Update Hall Pricing")
        .WithDescription("Updates an existing hall pricing configuration.")
        .Produces<ApiResponse<HallPricingDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);
       
        group.MapGet("/{hallPricingId:guid}", async (
            Guid hallPricingId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetHallPricingByIdQuery(hallPricingId),
                cancellationToken);

            return Results.Json(response,
                statusCode: response.StatusCode);
        })
        .WithName("GetHallPricingById")
        .WithSummary("Get Hall Pricing By Id")
        .WithDescription("Returns hall pricing by its identifier.")
        .Produces<ApiResponse<HallPricingDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetHallPricingQuery(request),               cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetHallPricings")
        .WithSummary("Get Hall Pricings")
        .WithDescription("Returns a paginated list of hall pricing configurations.")
        .Produces<ApiResponse<PaginatedResult<HallPricingDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/hall/{hallId:guid}/category/{eventCategoryId:guid}", async (
            Guid hallId,
            Guid eventCategoryId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send
            (
                new GetHallPricingByHallAndEventCategoryQuery(hallId,eventCategoryId),
                cancellationToken
            );
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetHallPricingByHallIdAndEventCategoryId")
        .WithSummary("Get Hall Pricing By Hall And Event Category")
        .WithDescription("Returns hall pricing for a specific hall and event category.")
        .Produces<ApiResponse<HallPricingDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
    }
}