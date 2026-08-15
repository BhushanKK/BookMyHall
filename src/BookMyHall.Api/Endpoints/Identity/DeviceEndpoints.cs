using MediatR;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;


namespace BookMyHall.Api.Endpoints.Identity;

public static class DeviceEndpoints
{
    public static void MapDeviceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/devices")
            .WithTags("Devices")
            .RequireAuthorization();

        group.MapPost("/", async (RegisterDeviceCommand command,IMediator mediator,CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("RegisterDevice")
        .WithSummary("Register Device")
        .WithDescription("Registers a new device for the authenticated user.")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{userId:guid}/{deviceIdentifier}", async (Guid userId,string deviceIdentifier,UpdateDeviceCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            command.UserId = userId;
            command.DeviceIdentifier = deviceIdentifier;
            var response = await mediator.Send(command, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("UpdateDevice")
        .WithSummary("Update Device")
        .WithDescription("Updates an existing device.")
        .Produces<ApiResponse<DeviceDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{userId:guid}/{deviceIdentifier}", async (Guid userId,string deviceIdentifier,IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new DeleteDeviceCommand(userId, deviceIdentifier),cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("DeleteDevice")
        .WithSummary("Delete Device")
        .WithDescription("Soft deletes a registered device.")
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async ([AsParameters] PaginationRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetDeviceQuery(request), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetDevices")
        .WithSummary("Get Devices")
        .WithDescription("Retrieves a paginated list of registered devices.")
        .Produces<ApiResponse<PaginatedResponse<DeviceDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/{deviceId:guid}", async (Guid deviceId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetByIdDeviceQuery(deviceId), cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithName("GetDeviceById")
        .WithSummary("Get Device By Id")
        .WithDescription("Retrieves a single device by its id.")
        .Produces<ApiResponse<DeviceDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
    }
}