using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateCountryCommandValidator
    : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.CountryName)
            .Required(localizer, EntityKeys.CountryName)
            .MaximumLength(100);

        RuleFor(x => x.CountryCode)
            .Required(localizer, EntityKeys.CountryCode)
            .MaximumLength(5);

        RuleFor(x => x.PhoneCode)
            .MaximumLength(10);

        RuleFor(x => x.CurrencyCode)
            .MaximumLength(10);
    }
}