using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetAmenityByIdQuery(Guid AmenityId)
    : IRequest<ApiResponse<AmenityDto>>;