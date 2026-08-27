using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Masters;

public class Country :BaseEntity
{
    public Guid CountryId {get;set;}
    public string CountryName {get;set;} =string.Empty; 
    public string CountryCode { get; set; } = string.Empty;
    public string? PhoneCode { get; set; }
    public string? CurrencyCode { get; set; }
    public bool IsActive { get; set; } 
    public bool IsDeleted { get; set; }
}