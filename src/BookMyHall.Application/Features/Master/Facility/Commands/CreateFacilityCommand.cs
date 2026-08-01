using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record CreateFacilityCommand(string FacilityName,string FacilityIcon)
    : IRequest<ApiResponse<Guid>>;