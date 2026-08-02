namespace BookMyHall.Application.Features.Authentication.Commands.ForgotPassword;

public sealed class ForgotPasswordResponse
{
    public string Message { get; init; } = string.Empty;
    public string? ResetToken { get; set; }
}