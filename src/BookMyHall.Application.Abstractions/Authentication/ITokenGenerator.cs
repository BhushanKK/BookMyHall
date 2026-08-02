namespace BookMyHall.Application.Abstractions.Authentication;

public interface ITokenGenerator
{
    string GeneratePasswordResetToken(int size = 32);
}