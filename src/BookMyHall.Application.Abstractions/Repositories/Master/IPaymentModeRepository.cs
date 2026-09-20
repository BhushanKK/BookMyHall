using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IPaymentModeRepository
{
    Task AddAsync(PaymentMode paymentMode,CancellationToken cancellationToken = default);

    Task UpdateAsync(PaymentMode paymentMode,CancellationToken cancellationToken = default);

    Task<PaymentMode?> GetByIdAsync(Guid paymentModeId,CancellationToken cancellationToken = default);

    Task<PaymentMode?> GetByPaymentModeNameAsync(string paymentModeName,CancellationToken cancellationToken = default);

    Task<PaginatedResult<PaymentMode>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, int limit = 20, CancellationToken cancellationToken = default);
}