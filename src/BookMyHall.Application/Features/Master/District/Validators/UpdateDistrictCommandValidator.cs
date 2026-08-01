using FluentValidation;
namespace BookMyHall.Application.Features.Master;

public sealed class UpdateDistrictCommandValidator
    : AbstractValidator<UpdateDistrictCommand>
{
    public UpdateDistrictCommandValidator()
    {
        RuleFor(x => x.DistrictId)
            .NotEmpty();

        RuleFor(x => x.StateId)
            .NotEmpty();

        RuleFor(x => x.DistrictName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.IsActive)
            .NotNull();
    }
}