using MediatR;

using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity.Users;
public sealed record GetUsersQuery(
    PaginationRequest Request)
    : IRequest<ApiResponse<PaginatedResponse<UserDto>>>;