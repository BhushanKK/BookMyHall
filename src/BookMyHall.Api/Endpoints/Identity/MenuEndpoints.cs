using MediatR;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Menu;

public static class MenuEndpoints
{
    public static void MapMenuEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/menus")
            .WithTags("Menu")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async(
            CreateMenuCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command,cancellationToken);
            return Results.Json(response,statusCode: response.StatusCode);
        })
        .WithName("CreateMenu")
        .WithSummary("Create Menu")
        .WithDescription("Creates a new menu.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{menuId:guid}", async (
            Guid menuId,
            UpdateMenuCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.MenuId = menuId;
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateMenu")
        .WithSummary("Update Menu")
        .WithDescription("Updates an existing menu.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{menuId:guid}", async (
            Guid menuId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeleteMenuCommand(menuId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteMenu")
        .WithSummary("Delete Menu")
        .WithDescription("Deletes an existing menu.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{menuId:guid}", async (Guid menuId,
            IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetByIdMenuQuery(menuId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetByIdMenu")
        .WithSummary("Get Menu By Id")
        .WithDescription("Returns a menu by its identifier.")
        .Produces<ApiResponse<MenuDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (IMediator mediator,CancellationToken cancellationToken) 
        =>  {
                var response = await mediator.Send(new GetMenuQuery(), cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
        .WithName("GetMenu")
        .WithSummary("Get Menus")
        .WithDescription("Returns all active menus.")
        .Produces<ApiResponse<IReadOnlyList<MenuDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
    