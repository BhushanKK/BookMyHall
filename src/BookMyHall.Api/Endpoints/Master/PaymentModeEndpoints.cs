using MediatR;
using BookMyHall.Application.Features.Master;

namespace BookMyHall.Api.Endpoints.Master;

public static class PaymentModeEndpoints
{
    public static void MapPaymentModeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payment-modes")
            .WithTags("Payment Modes")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreatePaymentModeCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{paymentModeId:guid}", async (
            Guid paymentModeId,
            UpdatePaymentModeCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.PaymentModeId = paymentModeId;

            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{paymentModeId:guid}", async (
            Guid paymentModeId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeletePaymentModeCommand(paymentModeId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapGet("/{paymentModeId:guid}", async (
            Guid paymentModeId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetPaymentModeByIdQuery(paymentModeId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapPost("/search", async (
            GetPaymentModesQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }
}