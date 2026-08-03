using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class CreateFacilityCommandValidator: AbstractValidator<CreateFacilityCommand>
{
    public CreateFacilityCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.FacilityName)
            .Required(localizer, EntityKeys.FacilityName)
            .MaximumLength(100);

        RuleFor(x => x.FacilityIcon)
            .Required(localizer, EntityKeys.FacilityIcon)
            .MaximumLength(500);
    }
}