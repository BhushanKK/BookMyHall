using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Abstractions.Persistence.Identity;

public interface IPasswordResetTokenRepository
{
    Task AddAsync(PasswordResetToken passwordResetToken,CancellationToken cancellationToken = default);
    Task<PasswordResetToken?> GetActiveTokenAsync(Guid userId,string tokenHash,CancellationToken cancellationToken = default);
    Task<IEnumerable<PasswordResetToken>> GetActiveTokensByUserIdAsync(Guid userId,CancellationToken cancellationToken = default);
    Task DeleteAsync(PasswordResetToken passwordResetToken,CancellationToken cancellationToken = default);
    Task DeleteByUserIdAsync(Guid userId,CancellationToken cancellationToken = default);
    Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
}