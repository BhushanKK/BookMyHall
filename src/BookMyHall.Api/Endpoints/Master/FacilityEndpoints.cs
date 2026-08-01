using MediatR;
using BookMyHall.Application.Features.Master;

namespace BookMyHall.Api.Endpoints.Master;

public static class FacilityEndpoints
{
    public static void MapFacilityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/facilities")
            .WithTags("Facilities")
           .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateFacilityCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{facilityId:guid}", async (
            Guid facilityId,
            UpdateFacilityCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.FacilityId = facilityId;

            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{facilityId:guid}", async (
            Guid facilityId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteFacilityCommand(facilityId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapGet("/{facilityId:guid}", async (
            Guid facilityId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetFacilityByIdQuery(facilityId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapPost("/search", async (
            GetFacilitiesQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }
}