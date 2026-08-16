using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record DeleteUserPreferenceCommand(Guid UserPreferenceId)
: IRequest<ApiResponse<bool>>;