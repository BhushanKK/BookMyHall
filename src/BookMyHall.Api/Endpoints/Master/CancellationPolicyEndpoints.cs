using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class CancellationPolicyEndpoints
{
    public static void MapCancellationPolicyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cancellation-policies")
            .WithTags("Cancellation Policies")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", CreateCancellationPolicyAsync)
            .WithName("CreateCancellationPolicy")
            .WithSummary("Create Cancellation Policy")
            .WithDescription("Creates a new cancellation policy.")
            .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{cancellationPolicyId:guid}", UpdateCancellationPolicyAsync)
            .WithName("UpdateCancellationPolicy")
            .WithSummary("Update Cancellation Policy")
            .WithDescription("Updates an existing cancellation policy.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{cancellationPolicyId:guid}", DeleteCancellationPolicyAsync)
            .WithName("DeleteCancellationPolicy")
            .WithSummary("Delete Cancellation Policy")
            .WithDescription("Soft deletes a cancellation policy.")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{cancellationPolicyId:guid}", GetCancellationPolicyByIdAsync)
            .WithName("GetCancellationPolicyById")
            .WithSummary("Get Cancellation Policy By Id")
            .WithDescription("Retrieves a cancellation policy by its unique identifier.")
            .Produces<ApiResponse<CancellationPolicyDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<CancellationPolicyDto>>(StatusCodes.Status404NotFound);

        group.MapGet("/GetAllCancellationPolicies", GetCancellationPoliciesAsync)
            .WithName("GetCancellationPolicies")
            .WithSummary("Get Cancellation Policies")
            .WithDescription("Retrieves a paginated list of cancellation policies.")
            .Produces<ApiResponse<PaginatedResult<CancellationPolicyDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PaginatedResult<CancellationPolicyDto>>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateCancellationPolicyAsync(
        CreateCancellationPolicyCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> UpdateCancellationPolicyAsync(
        Guid cancellationPolicyId,
        UpdateCancellationPolicyCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        command.CancellationPolicyId = cancellationPolicyId;

        var response = await sender.Send(command, cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> DeleteCancellationPolicyAsync(
        Guid cancellationPolicyId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new DeleteCancellationPolicyCommand(cancellationPolicyId),
            cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> GetCancellationPolicyByIdAsync(
        Guid cancellationPolicyId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetCancellationPolicyByIdQuery(cancellationPolicyId),
            cancellationToken);

        return Results.Json(response, statusCode:response.StatusCode);
    }

    private static async Task<IResult> GetCancellationPoliciesAsync(
        GetCancellationPoliciesQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return Results.Json(response, statusCode:response.StatusCode);
    }
}