using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateMenuPermissionCommand()
    : MenuPermissionDto, IRequest<ApiResponse<MenuPermissionDto>>;