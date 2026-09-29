using MediatR;
using BookMyHall.Contracts.Common;
namespace BookMyHall.Application.Features.Venue;
public sealed record GetVendorPackagesAutoCompleteQuery(string? SearchTerm, int Limit = 20) 
    : IRequest<ApiResponse<IReadOnlyList<AutoCompleteItem>>>;
