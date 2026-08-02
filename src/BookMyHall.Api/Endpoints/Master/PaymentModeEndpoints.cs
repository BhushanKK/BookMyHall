using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class PaymentModeEndpoints
{
    public static void MapPaymentModeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payment-modes")
            .WithTags("Payment Modes")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", CreatePaymentModeAsync)
            .WithName("CreatePaymentMode")
            .WithSummary("Create Payment Mode")
            .WithDescription("Creates a new payment mode.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{paymentModeId:guid}", UpdatePaymentModeAsync)
            .WithName("UpdatePaymentMode")
            .WithSummary("Update Payment Mode")
            .WithDescription("Updates an existing payment mode.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{paymentModeId:guid}", DeletePaymentModeAsync)
            .WithName("DeletePaymentMode")
            .WithSummary("Delete Payment Mode")
            .WithDescription("Soft deletes a payment mode.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{paymentModeId:guid}", GetPaymentModeByIdAsync)
            .WithName("GetPaymentModeById")
            .WithSummary("Get Payment Mode By Id")
            .WithDescription("Retrieves a payment mode by its unique identifier.")
            .Produces<ApiResponse<PaymentModeDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaymentModeDto>>(StatusCodes.Status404NotFound);

        group.MapGet("/GetAllPaymentModes", GetPaymentModesAsync)
            .WithName("GetPaymentModes")
            .WithSummary("Get Payment Modes")
            .WithDescription("Retrieves a paginated list of payment modes.")
            .Produces<ApiResponse<PaginatedResult<PaymentModeDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaginatedResult<PaymentModeDto>>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreatePaymentModeAsync(
        CreatePaymentModeCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> UpdatePaymentModeAsync(
        Guid paymentModeId,
        UpdatePaymentModeCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        command.PaymentModeId = paymentModeId;

        var response = await sender.Send(command, cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> DeletePaymentModeAsync(
        Guid paymentModeId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new DeletePaymentModeCommand(paymentModeId),
            cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> GetPaymentModeByIdAsync(
        Guid paymentModeId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetPaymentModeByIdQuery(paymentModeId),
            cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> GetPaymentModesAsync(
        GetPaymentModesQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }
}