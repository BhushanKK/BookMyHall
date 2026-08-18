using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateUserPreferenceCommandValidator
    : AbstractValidator<UpsertUserPreferenceCommand>
{
    public CreateUserPreferenceCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.UserId)
            .Required(localizer, EntityKeys.UserId);

        RuleFor(x => x.CurrencyCode)
            .Required(localizer, EntityKeys.CurrencyCode)
            .MaximumLengthLocalized(
                localizer,
                EntityKeys.CurrencyCode,
                10);

        RuleFor(x => x.TimeZone)
            .Required(localizer, EntityKeys.TimeZone)
            .MaximumLengthLocalized(
                localizer,
                EntityKeys.TimeZone,
                100);

        RuleFor(x => x.DateFormat)
            .Required(localizer, EntityKeys.DateFormat)
            .MaximumLengthLocalized(
                localizer,
                EntityKeys.DateFormat,
                50);

        RuleFor(x => x.TimeFormat)
            .Required(localizer, EntityKeys.TimeFormat)
            .MaximumLengthLocalized(
                localizer,
                EntityKeys.TimeFormat,
                20);

        RuleFor(x => x.LanguageCode)
            .Required(localizer, EntityKeys.LanguageCode)
            .MaximumLengthLocalized(
                localizer,
                EntityKeys.LanguageCode,
                20);

        RuleFor(x => x.Theme)
            .Required(localizer, EntityKeys.Theme)
            .MaximumLengthLocalized(
                localizer,
                EntityKeys.Theme,
                50);
    }
}