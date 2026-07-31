using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetAreaByIdQuery(Guid AreaId)
    : IRequest<ApiResponse<AreaDto>>;