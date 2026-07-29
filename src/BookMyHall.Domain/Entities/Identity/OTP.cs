using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Identity;
public class RolePermission : BaseEntity
{
    public Guid OTPId { get; set; }
    public Guid UserId { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string OTPCode { get; set; } = string.Empty;
    public string OTPType { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset VerifiedAt { get; set; }
}