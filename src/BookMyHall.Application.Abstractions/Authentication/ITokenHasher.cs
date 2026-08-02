namespace BookMyHall.Application.Abstractions.Authentication;

public interface ITokenHasher
{
    string Hash(string value);
}