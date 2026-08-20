using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Enums;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string? MiddleName,
    string? LastName,
    string MobileNumber,
    DateTimeOffset? DateOfBirth,
    Gender? Gender,
    string? EmailAddress,
    Guid RoleId)
    : IRequest<ApiResponse<UserDto>>;

public sealed record UpdateUserRequest(
    string FirstName,
    string? MiddleName,
    string? LastName,
    string MobileNumber,
    DateTimeOffset? DateOfBirth,
    Gender? Gender,
    string? EmailAddress,
    Guid RoleId);
