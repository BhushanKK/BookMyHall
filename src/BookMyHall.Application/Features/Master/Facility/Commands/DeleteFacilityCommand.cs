using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record DeleteFacilityCommand(Guid FacilityId)
    : IRequest<ApiResponse<bool>>;