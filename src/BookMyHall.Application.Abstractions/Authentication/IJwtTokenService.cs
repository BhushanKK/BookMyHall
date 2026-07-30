namespace BookMyHall.Application.Abstractions.Authentication;

public interface IJwtTokenService
{
    JwtTokenResult GenerateToken(JwtUser user);
}