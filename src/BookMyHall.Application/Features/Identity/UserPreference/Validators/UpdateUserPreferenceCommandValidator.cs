using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateUserPreferenceCommandValidator: AbstractValidator<UpdateUserPreferenceCommand>
{
    public UpdateUserPreferenceCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.UserId)
            .Required(localizer,EntityKeys.UserId);

        RuleFor(x => x.CurrencyCode)
            .Required(localizer,EntityKeys.CurrencyCode)
            .Length(3)
            .WithMessage("Currency code must be exactly 3 characters.");

        RuleFor(x => x.TimeZone)
            .Required(localizer,EntityKeys.TimeZone)
            .MaximumLengthLocalized(localizer,EntityKeys.TimeZone,50);

        RuleFor(x => x.DateFormat)
            .Required(localizer,EntityKeys.DateFormat)
            .MaximumLengthLocalized(localizer,EntityKeys.DateFormat,20);

        RuleFor(x => x.TimeFormat)
            .Required(localizer,EntityKeys.TimeFormat)
            .MaximumLengthLocalized(localizer,EntityKeys.TimeFormat,10);

        RuleFor(x => x.LanguageCode)
            .Required(localizer,EntityKeys.LanguageCode)
            .MaximumLengthLocalized(localizer,EntityKeys.LanguageCode,10);

        RuleFor(x => x.Theme)
            .Required(localizer,EntityKeys.Theme)
            .MaximumLengthLocalized(localizer,EntityKeys.Theme,20);
    }
}