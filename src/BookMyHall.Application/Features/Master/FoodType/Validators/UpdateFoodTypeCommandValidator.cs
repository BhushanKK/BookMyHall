using FluentValidation;
namespace BookMyHall.Application.Features.Master;
public sealed class UpdateFoodTypeCommandValidator: AbstractValidator<UpdateFoodTypeCommand>
{
    public UpdateFoodTypeCommandValidator()
    {
        RuleFor(x => x.FoodTypeId)
            .NotEmpty();

        RuleFor(x => x.FoodTypeName)
            .NotEmpty()
            .MaximumLength(100);
    }
}