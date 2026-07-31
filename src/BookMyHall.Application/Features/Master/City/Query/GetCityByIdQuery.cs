using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetCityByIdQuery(Guid CityId)
    : IRequest<ApiResponse<CityDto>>;