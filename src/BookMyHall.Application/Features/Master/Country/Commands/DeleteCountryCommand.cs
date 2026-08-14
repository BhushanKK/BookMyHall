using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record DeleteCountryCommand(Guid CountryId)
: IRequest<ApiResponse<bool>>;