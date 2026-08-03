using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;
public sealed class UpdateFoodTypeCommandValidator: AbstractValidator<UpdateFoodTypeCommand>
{
    public UpdateFoodTypeCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.FoodTypeId)
            .Required(localizer, EntityKeys.FoodTypeId);

        RuleFor(x => x.FoodTypeName)
            .Required(localizer, EntityKeys.FoodTypeName)
            .MaximumLength(100);
    }
}