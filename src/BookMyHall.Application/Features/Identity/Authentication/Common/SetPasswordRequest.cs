namespace BookMyHall.Application.Features.Authentication.Commands.SetPassword;

public sealed class SetPasswordRequest
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}