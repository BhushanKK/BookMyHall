using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Enums;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed record CreateUserCommand(
    string FirstName,
    string? MiddleName,
    string? LastName,
    string MobileNumber,
    DateTimeOffset DateOfBirth,
    Gender Gender,
    string? EmailAddress,
    string Password,
    Guid RoleId)
    : IRequest<ApiResponse<UserDto>>;