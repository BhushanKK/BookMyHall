using MediatR;

using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Features.Identity.Users;
public sealed record GetUsersQuery(
    PaginationRequest paginationRequest)
    : IRequest<ApiResponse<PaginatedResponse<UserDto>>>;