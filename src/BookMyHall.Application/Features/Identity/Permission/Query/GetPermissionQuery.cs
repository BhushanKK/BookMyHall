using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetPermissionQuery(PaginationRequest paginationRequest)
:IRequest<ApiResponse<PaginatedResponse<Permission>>>;