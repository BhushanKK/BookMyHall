using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateDistrictCommand 
:DistrictDto, IRequest<ApiResponse<DistrictDto>>;