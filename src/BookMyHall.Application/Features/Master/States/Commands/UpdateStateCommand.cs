using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateStateCommand()
    : StateDto,IRequest<ApiResponse<StateDto>>;