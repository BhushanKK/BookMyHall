using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string? MiddleName,
    string? LastName,
    string MobileNumber,
    string? EmailAddress,
    Guid RoleId)
    : IRequest<ApiResponse<UserDto>>;