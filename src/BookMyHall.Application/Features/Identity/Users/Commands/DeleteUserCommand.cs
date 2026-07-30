using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed record DeleteUserCommand(Guid UserId)
    : IRequest<ApiResponse<bool>>;