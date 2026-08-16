using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateUserPreferenceCommand
: UserPreferenceDto, IRequest<ApiResponse<UserPreferenceDto>>;