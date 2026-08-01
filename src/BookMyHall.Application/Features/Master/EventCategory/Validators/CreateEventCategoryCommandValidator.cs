using FluentValidation;
namespace BookMyHall.Application.Features.Master;
public sealed class CreateEventCategoryCommandValidator: AbstractValidator<CreateEventCategoryCommand>
{
    public CreateEventCategoryCommandValidator()
    {
        RuleFor(x => x.EventCategoryName)
            .NotEmpty()
            .MaximumLength(100);
    }
}