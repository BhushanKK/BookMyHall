using System.Text.Json.Serialization;

using BookMyHall.Domain.Enums;

namespace BookMyHall.Application.Features.Identity.Users;

public sealed class UserDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string FullName =>
        string.Join(" ",
            new[]
            {
                FirstName,
                MiddleName,
                LastName
            }
            .Where(x => !string.IsNullOrWhiteSpace(x)));
    public string MobileNumber { get; set; } = string.Empty;
     public string? ProfileImageUrl { get; set; } = string.Empty;
     
    public DateTimeOffset? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string? EmailAddress { get; set; }
    public bool IsActive { get; set; }
    [JsonIgnore]
    public IReadOnlyList<string> Roles { get; set; } = [];
}