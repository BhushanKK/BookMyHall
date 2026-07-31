using MediatR;
using BookMyHall.Application.Features.Master;
namespace BookMyHall.Api.Endpoints.Master;
public static class AreaEndpoints
{
    public static void MapAreaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/areas")
            .WithTags("Areas")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateAreaCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{areaId:guid}", async (
            Guid areaId,
            UpdateAreaCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.AreaId = areaId;
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{areaId:guid}", async (
            Guid areaId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteAreaCommand(areaId),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapGet("/{areaId:guid}", async (
            Guid areaId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetAreaByIdQuery(areaId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapPost("/search", async (
            GetAreasQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }
}