using MediatR;
using BookMyHall.Application.Features.Master;

namespace BookMyHall.Api.Endpoints.Master;

public static class CancellationPolicyEndpoints
{
    public static void MapCancellationPolicyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cancellation-policies")
            .WithTags("Cancellation Policies")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));


        group.MapPost("/", async (
            CreateCancellationPolicyCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{cancellationPolicyId:guid}", async (
            Guid cancellationPolicyId,
            UpdateCancellationPolicyCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.CancellationPolicyId = cancellationPolicyId;
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{cancellationPolicyId:guid}", async (
            Guid cancellationPolicyId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteCancellationPolicyCommand(cancellationPolicyId),cancellationToken);
            return Results.Ok(result);
        });

        group.MapGet("/{cancellationPolicyId:guid}", async (
            Guid cancellationPolicyId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetCancellationPolicyByIdQuery(cancellationPolicyId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapPost("/search", async (
            GetCancellationPoliciesQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }
}