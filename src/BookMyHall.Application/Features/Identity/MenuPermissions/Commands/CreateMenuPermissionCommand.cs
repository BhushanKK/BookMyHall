using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record CreateMenuPermissionCommand
(Guid MenuId ,Guid PermissionId)
: IRequest<ApiResponse<MenuPermissionDto>>;