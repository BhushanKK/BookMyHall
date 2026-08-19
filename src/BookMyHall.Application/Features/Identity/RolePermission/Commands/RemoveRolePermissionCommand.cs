using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record RemoveRolePermissionCommand(Guid RoleId,Guid PermissionId)
    : IRequest<ApiResponse<bool>>;