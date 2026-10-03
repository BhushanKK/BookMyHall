using BookMyHall.Contracts.Common;
using MediatR;

namespace BookMyHall.Application.Features.Venue;

public sealed record GetVendorOwnersAutoCompleteQuery(string? SearchTerm, int Limit = 20)
    : IRequest<ApiResponse<IReadOnlyList<AutoCompleteItem>>>;
