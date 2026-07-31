using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record DeleteCityCommand(Guid CityId)
    : IRequest<ApiResponse<bool>>;