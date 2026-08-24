using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetMenuRolePermissionQuery(
    PaginationRequest PaginationRequest)
    : IRequest<ApiResponse<PaginatedResponse<MenuRolePermissionDto>>>;