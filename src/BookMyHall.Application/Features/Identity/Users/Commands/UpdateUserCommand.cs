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
string? EmailAddress,
DateTimeOffset? DateOfBirth,
Gender? Gender,
bool IsActive,
List<Guid> Roles)
: IRequest<ApiResponse<UserDto>>;
