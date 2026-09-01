namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class SetPasswordResponse
{
    public string Message { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public bool PasswordSet { get; set; }
}