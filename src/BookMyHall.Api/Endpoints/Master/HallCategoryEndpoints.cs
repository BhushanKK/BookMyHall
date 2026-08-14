using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Api.Endpoints.Master;

public static class HallCategoryEndpoints
{
    public static void MapHallCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/hall-categories")
            .WithTags("Hall Category")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateHallCategoryCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command,cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("CreateHallCategory")
        .WithSummary("Create Hall Category")
        .WithDescription("Creates a new hall category.")
        .Produces<ApiResponse<HallCategoryDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{hallCategoryId:guid}", async (
            Guid hallCategoryId,
            UpdateHallCategoryCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.HallCategoryId = hallCategoryId;
            var response = await mediator.Send(command,cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("UpdateHallCategory")
        .WithSummary("Update Hall Category")
        .WithDescription("Updates an existing hall category.")
        .Produces<ApiResponse<HallCategoryDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{hallCategoryId:guid}", async (
            Guid hallCategoryId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeleteHallCategoryCommand(hallCategoryId),cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("DeleteHallCategory")
        .WithSummary("Delete Hall Category")
        .WithDescription("Deletes an existing hall category.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{hallCategoryId:guid}", async (
            Guid hallCategoryId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetHallCategoryByIdQuery(hallCategoryId),cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetHallCategoryById")
        .WithSummary("Get Hall Category By Id")
        .WithDescription("Returns a hall category by its identifier.")
        .Produces<ApiResponse<HallCategoryDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetHallCategoriesQuery(request),cancellationToken);

            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("GetHallCategories")
        .WithSummary("Get Hall Categories")
        .WithDescription("Returns a paginated list of hall categories.")
        .Produces<ApiResponse<PaginatedResponse<HallCategoryDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}