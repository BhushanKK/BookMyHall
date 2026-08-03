using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class CreateAreaCommandValidator: AbstractValidator<CreateAreaCommand>
{
    public CreateAreaCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.AreaName)
            .Required(localizer, EntityKeys.Area)
            .MaximumLengthLocalized(localizer, EntityKeys.Area, 150);

        RuleFor(x => x.Pincode)
            .Required(localizer, EntityKeys.Pincode)
            .Length(6);

        RuleFor(x => x.CityId)
            .Required(localizer, EntityKeys.CityId);
    }
}