using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Application.Features.Identity.Authentication;

namespace BookMyHall.Application.Features.Authentication.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(
    Guid UserId,
    string Token)
    : IRequest<ApiResponse<VerifyEmailResponse>>;