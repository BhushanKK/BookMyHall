using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record CreateCityCommand(Guid DistrictId,string CityName)
    : IRequest<ApiResponse<Guid>>;