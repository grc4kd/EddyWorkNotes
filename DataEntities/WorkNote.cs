using System.Text.Json.Serialization;

namespace DataEntities;

public record WorkNote(string Description = "",
                       [property: JsonPropertyName("id")] int? Id = null!)
{
    [property: JsonPropertyName("description")] 
    public string? Description { get; set; } = Description;

    [property: JsonPropertyName("recordedAtTimeUtc")]
    public DateTime RecordedAtTimeUtc { get; set; } = DateTime.UtcNow;
}

[JsonSerializable(typeof(List<WorkNote>))]
public sealed partial class WorkNoteSerializerContext : JsonSerializerContext
{

}