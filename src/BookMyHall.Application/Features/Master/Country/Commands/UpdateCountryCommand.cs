using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateCountryCommand()
    : CountryDto, IRequest<ApiResponse<CountryDto>>;