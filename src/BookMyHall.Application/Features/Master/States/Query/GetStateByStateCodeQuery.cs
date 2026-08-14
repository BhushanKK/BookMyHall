using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetStateByStateCodeQuery(string StateCode): IRequest<ApiResponse<StateDto>>;