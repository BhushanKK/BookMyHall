namespace BookMyHall.Application.Features.Identity.Login;

public sealed class LoginResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}