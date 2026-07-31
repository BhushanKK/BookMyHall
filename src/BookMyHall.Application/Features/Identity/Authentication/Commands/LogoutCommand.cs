using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed record LogoutCommand(string RefreshToken)
    : IRequest<ApiResponse<bool>>;