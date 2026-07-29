using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetRoleByIdQuery(Guid RoleId) 
    : IRequest<ApiResponse<RoleDto>>;