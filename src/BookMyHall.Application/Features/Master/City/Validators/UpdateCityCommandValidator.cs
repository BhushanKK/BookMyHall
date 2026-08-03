using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class UpdateCityCommandValidator
    : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.CityId)
            .Required(localizer, EntityKeys.CityId);

        RuleFor(x => x.DistrictId)
            .Required(localizer, EntityKeys.DistrictId);

        RuleFor(x => x.CityName)
            .Required(localizer, EntityKeys.CityName)
            .MaximumLength(100);
    }
}