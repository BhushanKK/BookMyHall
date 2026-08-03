using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class CreateCityCommandValidator
    : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.DistrictId)
            .Required(localizer, EntityKeys.DistrictId);

        RuleFor(x => x.CityName)
            .Required(localizer, EntityKeys.CityName)
            .MaximumLength(100);
    }
}