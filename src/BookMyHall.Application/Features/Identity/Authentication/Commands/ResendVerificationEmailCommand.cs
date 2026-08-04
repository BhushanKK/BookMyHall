using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Authentication.Commands.ResendVerificationEmail;

public sealed record ResendVerificationEmailCommand(
    string Email)
    : IRequest<ApiResponse<ResendVerificationEmailResponse>>;