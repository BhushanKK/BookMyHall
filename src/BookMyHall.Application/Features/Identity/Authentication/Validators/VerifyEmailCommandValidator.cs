using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Features.Authentication.Commands.VerifyEmail;

public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.Token).Required(localizer, EntityKeys.Token);
    }
}