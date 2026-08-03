using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class UpdateFacilityCommandValidator: AbstractValidator<UpdateFacilityCommand>
{
    public UpdateFacilityCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.FacilityId)
            .Required(localizer, EntityKeys.FacilityId);

        RuleFor(x => x.FacilityName)
            .Required(localizer, EntityKeys.FacilityName)
            .MaximumLength(100);

        RuleFor(x => x.FacilityIcon)
            .Required(localizer, EntityKeys.FacilityIcon)
            .MaximumLength(500);
    }
}