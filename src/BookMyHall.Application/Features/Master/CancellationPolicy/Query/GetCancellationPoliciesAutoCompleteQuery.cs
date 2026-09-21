using MediatR;
using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Features.Master;

public sealed record GetCancellationPoliciesAutoCompleteQuery(string? SearchTerm = null, int Limit = 20)
    : IRequest<ApiResponse<IReadOnlyList<AutoCompleteItem>>>;