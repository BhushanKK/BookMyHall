using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetFacilityByIdQuery(Guid FacilityId)
    : IRequest<ApiResponse<FacilityDto>>;