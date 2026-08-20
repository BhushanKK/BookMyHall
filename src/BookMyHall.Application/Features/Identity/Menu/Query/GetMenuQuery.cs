using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetMenuQuery(PaginationRequest Request)
    : IRequest<ApiResponse<PaginatedResponse<Menu>>>;