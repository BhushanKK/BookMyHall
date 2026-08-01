using FluentValidation;
namespace BookMyHall.Application.Features.Master;
public sealed class CreateFoodTypeCommandValidator: AbstractValidator<CreateFoodTypeCommand>
{
    public CreateFoodTypeCommandValidator()
    {
        RuleFor(x => x.FoodTypeName)
            .NotEmpty()
            .MaximumLength(100);
    }
}