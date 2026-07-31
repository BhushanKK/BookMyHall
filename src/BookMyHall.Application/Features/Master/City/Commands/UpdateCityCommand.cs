using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Master;

public sealed class UpdateCityCommand()
    :CityDto ,IRequest<ApiResponse<CityDto>>;

    
