using System.Text.Json.Serialization;

using BookMyHall.Application.Common.Json;

namespace BookMyHall.Application.Features.Identity;

public class MenuPermissionDto
{
    [JsonIgnore]
    public Guid MenuPermissionId {get;set;}
    [JsonConverter(typeof(NullableGuidJsonConverter))]
    public Guid? MenuId {get;set;}
    [JsonConverter(typeof(NullableGuidJsonConverter))]
    public Guid? PermissionId{get;set;}
}