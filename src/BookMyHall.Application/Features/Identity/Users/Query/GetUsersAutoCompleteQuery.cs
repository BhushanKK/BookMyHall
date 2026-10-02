using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Identity;
public sealed record GetUsersAutoCompleteQuery(string? SearchTerm, int Limit = 20) 
    : IRequest<ApiResponse<IReadOnlyList<AutoCompleteItem>>>;
