using FluentValidation;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateAreaCommandValidator
    : AbstractValidator<UpdateAreaCommand>
{
    public UpdateAreaCommandValidator()
    {
        RuleFor(x => x.AreaId)
            .NotEmpty();

        RuleFor(x => x.AreaName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Pincode)
            .NotEmpty()
            .Length(6);

        RuleFor(x => x.CityId)
            .NotEmpty();
    }
}