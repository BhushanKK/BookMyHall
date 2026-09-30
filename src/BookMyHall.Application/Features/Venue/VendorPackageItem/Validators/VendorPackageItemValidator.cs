using FluentValidation;
namespace BookMyHall.Application.Features.Venue.VendorPackageItems;

public sealed class CreateVendorPackageItemCommandValidator: AbstractValidator<CreateVendorPackageItemCommand>
{
    public CreateVendorPackageItemCommandValidator()
    {
        RuleFor(x => x.VendorPackageId)
            .NotEmpty()
            .WithMessage("Vendor package is required.");

        RuleFor(x => x.VendorServiceId)
            .NotEmpty()
            .WithMessage("Vendor service is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order cannot be negative.");
    }
}

public sealed class UpdateVendorPackageItemCommandValidator: AbstractValidator<UpdateVendorPackageItemCommand>
{
    public UpdateVendorPackageItemCommandValidator()
    {
        RuleFor(x => x.VendorPackageItemId)
            .NotEmpty()
            .WithMessage("Vendor package item is required.");

        RuleFor(x => x.VendorPackageId)
            .NotEmpty()
            .WithMessage("Vendor package is required.");

        RuleFor(x => x.VendorServiceId)
            .NotEmpty()
            .WithMessage("Vendor service is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order cannot be negative.");
    }
}

public sealed class GetVendorPackageItemsQueryValidator: AbstractValidator<GetVendorPackageItemsQuery>
{
    public GetVendorPackageItemsQueryValidator()
    {
        RuleFor(x => x.Pagination.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.Pagination.PageSize)
            .GreaterThan(0);
    }
}