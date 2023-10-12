using System.Text.Json.Serialization;

namespace TusBlazorClient;

public class TusPreviousUpload
{
    [JsonPropertyName("size")]
    public long? Size { get; init; }
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; init; }
    [JsonPropertyName("creationTime")]
    public string CreationTime { get; init; }
    [JsonPropertyName("urlStorageKey")]
    public string UrlStorageKey { get; init; }
}