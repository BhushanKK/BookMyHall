using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed record GetUserByIdQuery(Guid UserId)
    : IRequest<ApiResponse<UserDto>>;