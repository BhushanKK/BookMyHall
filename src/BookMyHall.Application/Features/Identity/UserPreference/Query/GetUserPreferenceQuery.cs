using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetUserPreferenceQuery(PaginationRequest Request)
    : IRequest<ApiResponse<PaginatedResponse<UserPreferenceDto>>>;