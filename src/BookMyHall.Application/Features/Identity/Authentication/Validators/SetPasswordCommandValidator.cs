using FluentValidation;

namespace BookMyHall.Application.Features.Authentication.Commands.SetPassword;

public sealed class SetPasswordCommandValidator
    : AbstractValidator<SetPasswordCommand>
{
    public SetPasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage(
                "Password must be at least 8 characters long.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage(
                "Confirm password is required.");

        RuleFor(x => x)
            .Must(x =>
                x.NewPassword == x.ConfirmPassword)
            .WithMessage(
                "Password and confirm password must match.");
    }
}