using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record CreateAmenityCommand(string AmenityName,string AmenityIcon)
    : IRequest<ApiResponse<Guid>>;