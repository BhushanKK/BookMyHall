using MediatR;
using BookMyHall.Application.Features.Master;

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
        });

        group.MapPut("/{serviceId:guid}", async (
            Guid serviceId,
            UpdateServiceCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.ServiceId = serviceId;

            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{serviceId:guid}", async (
            Guid serviceId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteServiceCommand(serviceId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapGet("/{serviceId:guid}", async (
            Guid serviceId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetServiceByIdQuery(serviceId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapPost("/search", async (
            GetServicesQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }
}