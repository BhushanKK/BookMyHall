using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record CreateAreaCommand(
    string AreaName,
    string Pincode,
    Guid CityId)
    : IRequest<ApiResponse<Guid>>;