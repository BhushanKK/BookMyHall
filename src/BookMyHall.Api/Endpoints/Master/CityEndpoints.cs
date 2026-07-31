using MediatR;
using BookMyHall.Application.Features.Master;
namespace BookMyHall.Api.Endpoints.Master;
public static class CityEndpoints
{
    public static void MapCityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cities")
            .WithTags("Cities")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (CreateCityCommand command,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{cityId:guid}", async (Guid cityId,UpdateCityCommand command,IMediator mediator,CancellationToken cancellationToken) =>
        {
            command.CityId = cityId;
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{cityId:guid}", async (Guid cityId,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteCityCommand(cityId),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapGet("/{cityId:guid}", async (Guid cityId,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetCityByIdQuery(cityId),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapPost("/search", async (GetCitiesQuery query,IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }
}