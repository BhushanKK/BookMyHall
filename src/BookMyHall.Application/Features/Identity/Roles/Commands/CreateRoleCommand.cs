using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateRoleCommand
    : RoleDto,IRequest<ApiResponse<RoleDto>>;