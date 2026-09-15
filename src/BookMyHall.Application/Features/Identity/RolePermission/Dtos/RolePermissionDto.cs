using System.Text.Json.Serialization;

using BookMyHall.Application.Common.Json;

namespace BookMyHall.Application.Features.Identity;

public class RolePermissionDto
{
    [JsonIgnore]
    public Guid RolePermissionId { get; set; }
    [JsonConverter(typeof(NullableGuidJsonConverter))]
    public Guid? RoleId { get; set; }
    [JsonConverter(typeof(NullableGuidJsonConverter))]
    public Guid? PermissionId { get; set; }
}