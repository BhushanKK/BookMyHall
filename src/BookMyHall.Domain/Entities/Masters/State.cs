using BookMyHall.Domain.Common;
namespace BookMyHall.Domain.Masters;
public class State : BaseEntity
{
    public Guid StateId { get; set; }
    public string StateName { get; set; } = string.Empty;
    public string StateCode { get; set; } = string.Empty;
}