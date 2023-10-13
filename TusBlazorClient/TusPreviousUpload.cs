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

public class TusPreviousUploadRef
{
    public TusPreviousUploadRef(int index, TusPreviousUpload tusPreviousUpload)
    {
        Index = index;
        TusPreviousUpload = tusPreviousUpload;
    }

    public int Index { get; init; }
    public TusPreviousUpload TusPreviousUpload { get; init; }
}