using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record DeleteAmenityCommand(Guid AmenityId)
    : IRequest<ApiResponse<bool>>;