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

        group.MapPost("/", async (
            CreateCancellationPolicyCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("CreateCancellationPolicy")
        .WithSummary("Create Cancellation Policy")
        .WithDescription("Creates a new cancellation policy.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{cancellationPolicyId:guid}", async (
            Guid cancellationPolicyId,
            UpdateCancellationPolicyCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.CancellationPolicyId = cancellationPolicyId;

            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateCancellationPolicy")
        .WithSummary("Update Cancellation Policy")
        .WithDescription("Updates an existing cancellation policy.")
        .Produces<ApiResponse<CancellationPolicyDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        group.MapDelete("/{cancellationPolicyId:guid}", async (
            Guid cancellationPolicyId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new DeleteCancellationPolicyCommand(cancellationPolicyId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteCancellationPolicy")
        .WithSummary("Delete Cancellation Policy")
        .WithDescription("Deletes an existing cancellation policy.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{cancellationPolicyId:guid}", async (
            Guid cancellationPolicyId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetCancellationPolicyByIdQuery(cancellationPolicyId),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetCancellationPolicyById")
        .WithSummary("Get Cancellation Policy By Id")
        .WithDescription("Returns a cancellation policy by its identifier.")
        .Produces<ApiResponse<CancellationPolicyDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            [AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(
                new GetCancellationPoliciesQuery(request),
                cancellationToken);

            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetCancellationPolicies")
        .WithSummary("Get Cancellation Policies")
        .WithDescription("Returns a paginated list of cancellation policies.")
        .Produces<ApiResponse<PaginatedResult<CancellationPolicyDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}