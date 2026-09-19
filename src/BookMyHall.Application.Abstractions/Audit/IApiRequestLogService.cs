using BookMyHall.Domain.Audit;

namespace BookMyHall.Application.Abstractions.Audit;

public interface IApiRequestLogService
{
    Task LogAsync(
        ApiRequestLog log,
        CancellationToken cancellationToken);
}