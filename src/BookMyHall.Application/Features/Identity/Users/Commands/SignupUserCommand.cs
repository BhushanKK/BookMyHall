using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed record SignupUserCommand(
    string FirstName,
    string? MiddleName,
    string? LastName,
    string MobileNumber,
    string? EmailAddress,
    string Password)
    : IRequest<ApiResponse<UserDto>>;