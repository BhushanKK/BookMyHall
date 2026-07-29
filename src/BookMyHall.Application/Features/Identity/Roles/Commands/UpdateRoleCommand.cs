using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateRoleCommand()
    : RoleDto,IRequest<ApiResponse<RoleDto>>;