using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetAllMenuPermissionQuery(
    PaginationRequest Request)
    : IRequest<ApiResponse<PaginatedResponse<MenuPermissionDto>>>;