using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;
public sealed class CreateFoodTypeCommandValidator: AbstractValidator<CreateFoodTypeCommand>
{
    public CreateFoodTypeCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.FoodTypeName)
            .Required(localizer, EntityKeys.FoodTypeName)
            .MaximumLength(100);
    }
}