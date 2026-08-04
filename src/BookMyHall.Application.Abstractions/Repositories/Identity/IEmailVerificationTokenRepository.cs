using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Identity;

public interface IEmailVerificationTokenRepository
{
    Task AddAsync(EmailVerificationToken emailVerificationToken, CancellationToken cancellationToken = default);
    Task<EmailVerificationToken?> GetActiveTokenAsync(Guid userId,string tokenHash,CancellationToken cancellationToken = default);
    Task<IEnumerable<EmailVerificationToken>> GetActiveTokensByUserIdAsync(Guid userId,CancellationToken cancellationToken = default);
    Task DeleteAsync(EmailVerificationToken emailVerificationToken,CancellationToken cancellationToken = default);
    Task DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
}