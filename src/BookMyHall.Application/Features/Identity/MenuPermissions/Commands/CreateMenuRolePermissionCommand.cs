using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateMenuRolePermissionCommand
    : MenuRolePermissionDto,
      IRequest<ApiResponse<MenuRolePermissionDto>>;