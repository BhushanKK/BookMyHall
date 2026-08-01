using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IServiceRepository
{
    Task AddAsync(Service service,CancellationToken cancellationToken = default);

    Task UpdateAsync(Service service,CancellationToken cancellationToken = default);

    Task<Service?> GetByIdAsync(Guid serviceId,CancellationToken cancellationToken = default);

    Task<Service?> GetByServiceNameAsync(string serviceName,CancellationToken cancellationToken = default);

    Task<PaginatedResult<Service>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
}