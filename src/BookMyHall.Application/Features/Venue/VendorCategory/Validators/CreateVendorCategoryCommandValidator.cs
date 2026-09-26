using FluentValidation;
namespace BookMyHall.Application.Features.Venue;
public sealed class CreateVendorCategoryCommandValidator: AbstractValidator<CreateVendorCategoryCommand>
{
    public CreateVendorCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Vendor category name is required.")
            .MaximumLength(150)
            .WithMessage("Vendor category name cannot exceed 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order cannot be negative.");
    }
}