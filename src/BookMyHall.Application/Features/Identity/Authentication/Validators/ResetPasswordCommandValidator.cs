using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Features.Authentication.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.Token)
            .Required(localizer, "ResetToken");

        RuleFor(x => x.NewPassword)
            .Required(localizer, "NewPassword")
            .MinimumLengthLocalized(localizer, "NewPassword", 8)
            .StrongPasswordLocalized(localizer);

        RuleFor(x => x.ConfirmPassword)
            .Required(localizer, "ConfirmPassword")
            .EqualToLocalized(
                x => x.NewPassword,
                localizer,
                "ConfirmPassword",
                "NewPassword");
    }
}