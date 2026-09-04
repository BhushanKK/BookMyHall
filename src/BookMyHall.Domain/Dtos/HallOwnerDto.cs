namespace BookMyHall.Domain.Dtos;

public sealed class HallOwnerDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
}