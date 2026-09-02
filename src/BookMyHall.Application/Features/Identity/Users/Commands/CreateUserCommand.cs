using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed record CreateUserCommand(
    string FirstName,
    string? MiddleName,
    string? LastName,
    string MobileNumber,
    string? EmailAddress,
    List<Guid> Roles)
    : IRequest<ApiResponse<UserDto>>;