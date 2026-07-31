using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Features.Identity.Authentication;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.MobileNumber)
            .Required(localizer, EntityKeys.MobileNumber);

        RuleFor(x => x.Password)
            .Required(localizer, EntityKeys.Password);
    }
}