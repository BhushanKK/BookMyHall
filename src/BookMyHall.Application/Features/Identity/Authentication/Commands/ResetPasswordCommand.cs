using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Features.Identity.Authentication;

namespace BookMyHall.Application.Features.Authentication.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    Guid UserId,
    string Token,
    string NewPassword,
    string ConfirmPassword)
    : IRequest<ApiResponse<ResetPasswordResponse>>;