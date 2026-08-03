using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateAreaCommand()
    :AreaDto, IRequest<ApiResponse<AreaDto>>;
