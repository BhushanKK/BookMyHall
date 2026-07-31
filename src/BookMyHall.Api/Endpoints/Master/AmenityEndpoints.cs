using MediatR;
namespace BookMyHall.Application.Features.Master;

public static class AmenityEndpoints
{
    public static void MapAmenityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/amenities")
            .WithTags("Amenities")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/", async (CreateAmenityCommand command, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/{amenityId:guid}", async (Guid amenityId, UpdateAmenityCommand command, IMediator mediator, CancellationToken cancellationToken) =>
        {
            command.AmenityId = amenityId;
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/{amenityId:guid}", async (Guid amenityId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteAmenityCommand(amenityId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapGet("/{amenityId:guid}", async (Guid amenityId, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetAmenityByIdQuery(amenityId),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapPost("/search", async (GetAmenitiesQuery query, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        });
    }

    private static async Task<IResult> CreateAmenityAsync(CreateAmenityCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> UpdateAmenityAsync(Guid amenityId, UpdateAmenityCommand command, ISender sender, CancellationToken cancellationToken)
    {
        command.AmenityId = amenityId;
        var response = await sender.Send(command, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> DeleteAmenityAsync(Guid amenityId, ISender sender, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new DeleteAmenityCommand(amenityId), cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetAmenityByIdAsync(Guid amenityId, ISender sender, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetAmenityByIdQuery(amenityId), cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }

    private static async Task<IResult> GetAmenitiesAsync(GetAmenitiesQuery query, ISender sender, CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }
}