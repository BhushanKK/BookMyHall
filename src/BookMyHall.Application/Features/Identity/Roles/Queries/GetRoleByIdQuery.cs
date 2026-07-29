using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetRoleByIdQuery(Guid RoleId) 
    : IRequest<ApiResponse<Role>>;