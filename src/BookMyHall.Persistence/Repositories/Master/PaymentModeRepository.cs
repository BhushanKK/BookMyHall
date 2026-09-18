using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class PaymentModeRepository(BookMyHallDbContext context): IPaymentModeRepository
{
    public async Task AddAsync(PaymentMode paymentMode,CancellationToken cancellationToken = default)
        => await context.PaymentModes.AddAsync(paymentMode,cancellationToken);

    public Task UpdateAsync(PaymentMode paymentMode,CancellationToken cancellationToken = default)
    {
        context.PaymentModes.Update(paymentMode);
        return Task.CompletedTask;
    }

    public async Task<PaymentMode?> GetByIdAsync(Guid paymentModeId,CancellationToken cancellationToken = default)
        => await context.PaymentModes 
            .FirstOrDefaultAsync(x => x.PaymentModeId == paymentModeId &&!x.IsDeleted && x.IsActive,cancellationToken);

    public async Task<PaymentMode?> GetByPaymentModeNameAsync(string paymentModeName,CancellationToken cancellationToken = default)
        => await context.PaymentModes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PaymentModeName == paymentModeName &&!x.IsDeleted,cancellationToken);

    public async Task<PaginatedResult<PaymentMode>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.PaymentModes.AsNoTracking().Where(x=>!x.IsDeleted && x.IsActive);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern=$"%{search}%";
            query = query.Where(x =>EF.Functions.ILike(x.PaymentModeName,pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.PaymentModeName)
            .ThenBy(x=>x.PaymentModeId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<PaymentMode>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}