using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetRolesQuery(PaginationRequest Request)
    : IRequest<ApiResponse<PaginatedResponse<RoleDto>>>;