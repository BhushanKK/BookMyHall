using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Enums;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed record ProfileUpdateUserCommand(
    Guid UserId,
    string FirstName,
    string? MiddleName,
    string? LastName,
    string MobileNumber,
    DateTimeOffset? DateOfBirth,
    Gender? Gender,
    string EmailAddress,
    Stream? ImageStream = null,
    string? FileName = null,
    string? ContentType = null,
    long? FileSize = null
) : IRequest<ApiResponse<UserDto>>;