using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusHttpRequest
{
    [JsonPropertyName("method")]
    public string Method { get; init; }
    [JsonPropertyName("url")]
    public string Url { get; init; }
}