using MediatR;
using BookMyHall.Application.Features.Master;

namespace BookMyHall.Api.Endpoints.Master;

public static class StateEndpoints
{
    public static void MapStateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/states")
            .WithTags("States")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateStateCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command,cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{stateId:guid}", async (Guid stateId,
            UpdateStateCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.StateId=stateId;
            var result = await mediator.Send(command,cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{stateId:guid}", async (
            Guid stateId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteStateCommand(stateId),cancellationToken);
            return Results.Ok(result);
        });

        group.MapGet("/{stateId:guid}", async (
            Guid stateId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetStateByIdQuery(stateId),cancellationToken);
            return Results.Ok(result);
        });
    }
}