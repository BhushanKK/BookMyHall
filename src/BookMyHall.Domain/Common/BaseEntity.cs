using System.Text.Json.Serialization;

namespace BookMyHall.Domain.Common;

public abstract class BaseEntity
{
    [JsonIgnore]
    public Guid? CreatedBy { get; set; }
    [JsonIgnore]
    public DateTimeOffset CreatedDate { get; set; }
    [JsonIgnore]
    public Guid? UpdatedBy { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedDate { get; set; }
}

