using MediatR;
using BookMyHall.Application.Features.Master;
namespace BookMyHall.Api.Endpoints.Master;
public static class DistrictEndpoints
{
    public static void MapDistrictEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/districts")
            .WithTags("Districts")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (
            CreateDistrictCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{districtId:guid}", async (
            Guid districtId,
            UpdateDistrictCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.DistrictId = districtId;

            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{districtId:guid}", async (
            Guid districtId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteDistrictCommand(districtId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapGet("/{districtId:guid}", async (
            Guid districtId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetDistrictByIdQuery(districtId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapPost("/search", async (
            GetDistrictsQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }
}