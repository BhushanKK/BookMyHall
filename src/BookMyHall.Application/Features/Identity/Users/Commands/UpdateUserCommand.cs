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
    Guid RoleId,
    string EmailAddress,
    string? profileImageUrl, 
    Stream? ImageStream = null ,
    string? FileName = null,
    string? ContentType = null,
    long? FileSize = null)
    : IRequest<ApiResponse<UserDto>>;

public sealed record UpdateUserRequest(
    string FirstName,
    string? MiddleName,
    string? LastName,
    string MobileNumber,
    DateTimeOffset? DateOfBirth,
    Gender? Gender,
    string EmailAddress,
    Guid RoleId,
    string? profileImageUrl,
     Stream? ImageStream = null,
    string? FileName = null,
    string? ContentType = null,
    long? FileSize = null);
