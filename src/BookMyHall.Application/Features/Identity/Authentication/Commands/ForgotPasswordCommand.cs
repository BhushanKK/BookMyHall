using BookMyHall.Contracts.Common;

using MediatR;

namespace BookMyHall.Application.Features.Authentication.Commands.ForgotPassword;
public sealed record ForgotPasswordCommand(string Email)
    : IRequest<ApiResponse<ForgotPasswordResponse>>;
