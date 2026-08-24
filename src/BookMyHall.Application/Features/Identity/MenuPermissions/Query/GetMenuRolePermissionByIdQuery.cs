using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetMenuRolePermissionByIdQuery(
    Guid MenuRolePermissionId)
    : IRequest<ApiResponse<MenuRolePermissionDto>>;