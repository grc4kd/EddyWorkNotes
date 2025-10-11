using System.Text.Json.Serialization;

namespace DataEntities;

public class WorkNote
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("recordedAtTime")]
    public DateTime RecordedAtTime { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}


[JsonSerializable(typeof(List<WorkNote>))]
public sealed partial class WorkNoteSerializerContext : JsonSerializerContext
{
}