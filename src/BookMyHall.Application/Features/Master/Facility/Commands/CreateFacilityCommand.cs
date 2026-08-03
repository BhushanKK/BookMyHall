using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateFacilityCommand
    :FacilityDto, IRequest<ApiResponse<FacilityDto>>;