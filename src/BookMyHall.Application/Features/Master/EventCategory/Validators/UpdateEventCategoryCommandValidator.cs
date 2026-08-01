using FluentValidation;
namespace BookMyHall.Application.Features.Master;
public sealed class UpdateEventCategoryCommandValidator: AbstractValidator<UpdateEventCategoryCommand>
{
    public UpdateEventCategoryCommandValidator()
    {
        RuleFor(x => x.EventCategoryId)
            .NotEmpty();

        RuleFor(x => x.EventCategoryName)
            .NotEmpty()
            .MaximumLength(100);
    }
}