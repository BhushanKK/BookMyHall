using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateMenuRolePermissionCommand
    : MenuRolePermissionDto,
      IRequest<ApiResponse<MenuRolePermissionDto>>;