using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateServiceCommand()
    :ServiceDto, IRequest<ApiResponse<ServiceDto>>;