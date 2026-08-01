using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record CreateDistrictCommand(Guid StateId,string DistrictName) 
: IRequest<ApiResponse<Guid>>;