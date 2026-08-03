using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class PaymentModeEndpoints
{
    public static void MapPaymentModeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payment-modes")
            .WithTags("Payment Mode")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreatePaymentModeCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreatePaymentMode")
        .WithSummary("Create Payment Mode")
        .WithDescription("Creates a new payment mode.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{paymentModeId:guid}", async (
            Guid paymentModeId,
            UpdatePaymentModeCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.PaymentModeId = paymentModeId;

            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdatePaymentMode")
        .WithSummary("Update Payment Mode")
        .WithDescription("Updates an existing payment mode.")
        .Produces<ApiResponse<PaymentModeDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{paymentModeId:guid}", async (
            Guid paymentModeId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new DeletePaymentModeCommand(paymentModeId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeletePaymentMode")
        .WithSummary("Delete Payment Mode")
        .WithDescription("Deletes an existing payment mode.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{paymentModeId:guid}", async (
            Guid paymentModeId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetPaymentModeByIdQuery(paymentModeId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetPaymentModeById")
        .WithSummary("Get Payment Mode By Id")
        .WithDescription("Returns a payment mode by its identifier.")
        .Produces<ApiResponse<PaymentModeDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetPaymentModesQuery(request),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetPaymentModes")
        .WithSummary("Get Payment Modes")
        .WithDescription("Returns a paginated list of payment modes.")
        .Produces<ApiResponse<PaginatedResponse<PaymentModeDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}