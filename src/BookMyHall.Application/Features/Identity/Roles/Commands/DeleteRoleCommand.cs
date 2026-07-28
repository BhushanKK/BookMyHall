using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record DeleteRoleCommand(Guid RoleId)
    : IRequest<ApiResponse<bool>>;