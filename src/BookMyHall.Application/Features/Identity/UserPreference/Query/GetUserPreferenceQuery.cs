using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetUserPreferenceQuery(Guid UserId)
    : IRequest<ApiResponse<UserPreferenceDto>>;