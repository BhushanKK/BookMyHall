using MediatR;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Application.Features.Identity;

public sealed record GetUserDetailsByIdQuery(Guid userId,Guid roleId) 
    : IRequest<ApiResponse<UserDetailsView>>;