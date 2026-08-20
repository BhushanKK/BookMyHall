using FluentValidation;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class UpdateUserProfileImageCommandValidator
    : AbstractValidator<UpdateUserProfileImageCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public UpdateUserProfileImageCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.ImageStream)
            .NotNull()
            .WithMessage("Profile image is required.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("Profile image file name is required.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(contentType =>
                AllowedContentTypes.Contains(
                    contentType,
                    StringComparer.OrdinalIgnoreCase))
            .WithMessage(
                "Only JPG, PNG, and WEBP images are allowed.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(MaxFileSize)
            .WithMessage(
                "Profile image size must be between 1 byte and 5 MB.");
    }
}