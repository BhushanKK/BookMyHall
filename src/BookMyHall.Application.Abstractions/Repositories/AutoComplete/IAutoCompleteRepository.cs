using System.Linq.Expressions;

using BookMyHall.Contracts.Common;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;
public interface IAutoCompleteRepository
{
    Task<IReadOnlyList<AutoCompleteItem>> GetAsync<TEntity>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, Guid>> idSelector,
        Expression<Func<TEntity, string>> nameSelector,
        string? searchTerm,
        int limit = 20,
        CancellationToken cancellationToken = default)
        where TEntity : class;
}