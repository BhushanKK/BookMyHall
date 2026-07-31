namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}