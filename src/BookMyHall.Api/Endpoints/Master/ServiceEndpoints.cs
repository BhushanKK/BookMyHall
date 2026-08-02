using MediatR;
using BookMyHall.Application.Features.Master;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Api.Endpoints.Master;

public static class ServiceEndpoints
{
    public static void MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/services")
            .WithTags("Services")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateServiceCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("CreateService")
        .WithSummary("Create Service")
        .WithDescription("Creates a new service.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<Guid>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<Guid>>(StatusCodes.Status409Conflict);

        group.MapPut("/{serviceId:guid}", async (
            Guid serviceId,
            UpdateServiceCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.ServiceId = serviceId;

            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("UpdateService")
        .WithSummary("Update Service")
        .WithDescription("Updates an existing service.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<bool>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<bool>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{serviceId:guid}", async (
            Guid serviceId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteServiceCommand(serviceId),
                cancellationToken);

            return Results.Ok(result);
        })
        .WithName("DeleteService")
        .WithSummary("Delete Service")
        .WithDescription("Soft deletes a service.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapGet("/{serviceId:guid}", async (
            Guid serviceId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetServiceByIdQuery(serviceId),
                cancellationToken);

            return Results.Ok(result);
        })
        .WithName("GetServiceById")
        .WithSummary("Get Service By Id")
        .WithDescription("Retrieves a service by its unique identifier.")
        .Produces<ApiResponse<ServiceDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<ServiceDto>>(StatusCodes.Status404NotFound);

        group.MapGet("/GetAllServices", async (
            GetServicesQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetServices")
        .WithSummary("Get Services")
        .WithDescription("Retrieves a paginated list of services.")
        .Produces<ApiResponse<PaginatedResult<ServiceDto>>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<PaginatedResult<ServiceDto>>>(StatusCodes.Status400BadRequest);
    }
}