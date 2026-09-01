using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Features.Identity.Authentication;

namespace BookMyHall.Application.Features.Authentication.Commands.SetPassword;

public sealed record SetPasswordCommand(
    Guid UserId,
    string Token,
    string NewPassword,
    string ConfirmPassword)
    : IRequest<ApiResponse<SetPasswordResponse>>;