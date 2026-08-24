using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record UpsertMenuRolePermissionCommand(
    Guid RoleId,
    IReadOnlyList<MenuRolePermissionRequest> Permissions)
    : IRequest<ApiResponse<IReadOnlyList<MenuRolePermissionDto>>>;

public sealed record MenuRolePermissionRequest(
    Guid MenuId,
    bool CanView,
    bool CanCreate,
    bool CanUpdate,
    bool CanDelete,
    bool CanPrint,
    bool CanExport);