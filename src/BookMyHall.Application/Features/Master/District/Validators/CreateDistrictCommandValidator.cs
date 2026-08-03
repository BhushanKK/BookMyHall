using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class CreateDistrictCommandValidator: AbstractValidator<CreateDistrictCommand>
{
    public CreateDistrictCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.StateId)
            .Required(localizer, EntityKeys.StateId);

        RuleFor(x => x.DistrictName)
            .Required(localizer, EntityKeys.DistrictName)
            .MaximumLength(100);
    }
}