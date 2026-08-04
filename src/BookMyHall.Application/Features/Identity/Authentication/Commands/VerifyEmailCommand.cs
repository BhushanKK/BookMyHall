using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Authentication.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(
    string Email,
    string Token)
    : IRequest<ApiResponse<VerifyEmailResponse>>;