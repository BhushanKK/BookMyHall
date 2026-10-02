using FluentValidation;
namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateVendorSubCategoryCommandValidator: AbstractValidator<UpdateVendorSubCategoryCommand>
{
    public UpdateVendorSubCategoryCommandValidator()
    {
        RuleFor(x => x.VendorSubCategoryId)
            .NotEmpty()
            .WithMessage("Vendor sub category is required.");

        RuleFor(x => x.VendorCategoryId)
            .NotEmpty()
            .WithMessage("Vendor category is required.");

        RuleFor(x => x.VendorSubCategoryName)
            .NotEmpty()
            .WithMessage("Sub category name is required.")
            .MaximumLength(200)
            .WithMessage("Sub category name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order cannot be negative.");
    }
}