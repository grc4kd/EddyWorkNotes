using System.Text.Json.Serialization;

namespace DataEntities;

public record Contact(int Id, string Name, string? Email = "", string? Phone = "")
{
    [property: JsonPropertyName("id")]
    public int Id { get; set; } = Id;

    [property: JsonPropertyName("name")]
    public string Name { get; set; } = Name;

    [property: JsonPropertyName("email")]
    public string? Email { get; set; } = Email ?? string.Empty;

    [property: JsonPropertyName("phone")]
    public string? Phone { get; set; } = Phone ?? string.Empty;
}

[JsonSerializable(typeof(List<WorkNote>))]
public sealed partial class ContactSerializerContext : JsonSerializerContext
{

}