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
List<Guid> Roles,
bool IsActive=true)
: IRequest<ApiResponse<UserDto>>;
