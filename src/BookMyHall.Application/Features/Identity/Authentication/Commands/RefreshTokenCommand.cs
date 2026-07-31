using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed record RefreshTokenCommand(string RefreshToken)
    : IRequest<ApiResponse<LoginResponse>>;