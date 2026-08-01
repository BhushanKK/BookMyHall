using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;
public sealed class UpdateFacilityCommand()
    :FacilityDto ,IRequest<ApiResponse<FacilityDto>>;