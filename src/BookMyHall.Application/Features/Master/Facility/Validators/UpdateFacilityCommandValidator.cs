using FluentValidation;
namespace BookMyHall.Application.Features.Master;

public sealed class UpdateFacilityCommandValidator: AbstractValidator<UpdateFacilityCommand>
{
    public UpdateFacilityCommandValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty();

        RuleFor(x => x.FacilityName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.FacilityIcon)
            .NotEmpty()
            .MaximumLength(500);
    }
}