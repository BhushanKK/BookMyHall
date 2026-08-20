
using BookMyHall.Contracts.Common;

using MediatR;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed record UpdateUserProfileImageCommand(
    Guid UserId,
    Stream ImageStream,
    string FileName,
    string ContentType,
    long FileSize)
    : IRequest<ApiResponse<UserDto>>;