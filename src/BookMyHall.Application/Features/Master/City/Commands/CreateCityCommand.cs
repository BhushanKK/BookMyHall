using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateCityCommand
    :CityDto, IRequest<ApiResponse<CityDto>>;