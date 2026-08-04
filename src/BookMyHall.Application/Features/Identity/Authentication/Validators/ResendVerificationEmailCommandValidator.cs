using FluentValidation;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Features.Authentication.Commands.ResendVerificationEmail;

public sealed class ResendVerificationEmailCommandValidator
    : AbstractValidator<ResendVerificationEmailCommand>
{
    public ResendVerificationEmailCommandValidator(
        ILocalizationService localizationService)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}