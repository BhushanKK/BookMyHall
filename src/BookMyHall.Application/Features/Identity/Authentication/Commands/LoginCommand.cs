using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed record LoginCommand(string MobileNumber, string Password)
    : IRequest<ApiResponse<LoginResponse>>;