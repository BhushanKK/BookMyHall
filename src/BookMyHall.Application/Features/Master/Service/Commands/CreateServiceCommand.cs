using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateServiceCommand
    :ServiceDto, IRequest<ApiResponse<ServiceDto>>;