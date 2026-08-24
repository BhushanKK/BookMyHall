using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record DeleteMenuRolePermissionCommand(
    Guid MenuRolePermissionId)
    : IRequest<ApiResponse<bool>>;