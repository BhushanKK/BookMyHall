using System.Security.Cryptography;

using BookMyHall.Application.Abstractions.Authentication;

namespace BookMyHall.Infrastructure.Authentication;

public sealed class TokenGenerator : ITokenGenerator
{
    public string GeneratePasswordResetToken(int size = 32)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
        var bytes = RandomNumberGenerator.GetBytes(size);
        return Convert.ToHexString(bytes);
    }

    public string GenerateEmailVerificationToken() 
        => GenerateSecureToken();
    private static string GenerateSecureToken() 
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}