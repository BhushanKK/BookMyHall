using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetHallImageContentQuery(Guid HallImageId)
    : IRequest<HallImageContentResult?>;