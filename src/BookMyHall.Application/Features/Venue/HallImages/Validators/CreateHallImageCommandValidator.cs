using FluentValidation;

using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateHallImageCommandValidator
    : AbstractValidator<CreateHallImageCommand>
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    private static readonly string[] AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    ];

    public CreateHallImageCommandValidator(
        ILocalizationService localizer)
    {
        RuleFor(x => x.HallId)
            .Required(localizer, EntityKeys.HallId);

        RuleFor(x => x.ImageStream)
            .NotNull()
            .WithMessage("Image is required.");

        RuleFor(x => x.FileName)
            .Required(localizer, EntityKeys.FileName)
            .Must(HasAllowedExtension)
            .WithMessage(
                "Only JPG, JPEG, PNG and WEBP images are supported.");

        RuleFor(x => x.ContentType)
            .Required(localizer, EntityKeys.ContentType)
            .Must(IsAllowedContentType)
            .WithMessage(
                "Only JPG, JPEG, PNG and WEBP images are supported.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("Image file cannot be empty.")
            .LessThanOrEqualTo(MaxFileSize)
            .WithMessage("Image size cannot exceed 5 MB.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThan(0);
    }

    private static bool HasAllowedExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension = Path.GetExtension(fileName);

        return AllowedExtensions.Contains(
            extension,
            StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsAllowedContentType(string contentType)
    {
        return AllowedContentTypes.Contains(
            contentType,
            StringComparer.OrdinalIgnoreCase);
    }
}