using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Venue;
public sealed record GetHallsAutoCompleteQuery(string? SearchTerm = null, int Limit = 20)
    : IRequest<ApiResponse<IReadOnlyList<AutoCompleteItem>>>;