using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Domain.Entities.Identity;

using Microsoft.AspNetCore.Identity;

namespace BookMyHall.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(string password)
        => _passwordHasher.HashPassword(new User(), password);

    public bool VerifyPassword(string hashedPassword,string providedPassword)
    {
        try
        {
            var result = _passwordHasher.VerifyHashedPassword(new User(),hashedPassword,providedPassword);
            return result != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}