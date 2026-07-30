using FluentAssertions;
using BookMyHall.Infrastructure.Security;

namespace BookMyHall.Infrastructure.Tests.Security;

public sealed class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher = new();

    [Fact]
    public void HashPassword_Should_Return_Hashed_Password()
    {
        const string password = "Admin@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        hashedPassword.Should().NotBeNullOrWhiteSpace();
        hashedPassword.Should().NotBe(password);
    }

    [Fact]
    public void VerifyPassword_Should_Return_True_When_Password_Is_Correct()
    {
        const string password = "Admin@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(hashedPassword,password);
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_Should_Return_False_When_Password_Is_Incorrect()
    {
        const string password = "Admin@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(hashedPassword,"WrongPassword");
        result.Should().BeFalse();
    }

    [Fact]
    public void HashPassword_Should_Return_Different_Hash_For_Same_Password()
    {
        const string password = "Admin@123";
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyPassword_Should_Return_True_For_Both_Generated_Hashes()
    {
        const string password = "Admin@123";
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);
        var result1 = _passwordHasher.VerifyPassword(hash1, password);
        var result2 = _passwordHasher.VerifyPassword(hash2, password);
        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_Should_Return_False_For_Invalid_Hash()
    {
        const string invalidHash = "InvalidHash";
        const string password = "Admin@123";
        var result = _passwordHasher.VerifyPassword(invalidHash,password);
        result.Should().BeFalse();
    }
}