namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}