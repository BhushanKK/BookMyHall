using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed record GetCountryByIdQuery(Guid CountryId)
    : IRequest<ApiResponse<Country>>;