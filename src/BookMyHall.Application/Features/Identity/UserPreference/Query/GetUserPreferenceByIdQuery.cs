using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetUserPreferenceByIdQuery(Guid UserPreferenceId,Guid UserId)
: IRequest<ApiResponse<UserPreference>>;