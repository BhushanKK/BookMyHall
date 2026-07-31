namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class LoginRequest
{
    public string MobileNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}