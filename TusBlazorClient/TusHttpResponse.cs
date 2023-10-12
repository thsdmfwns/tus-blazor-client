using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusHttpResponse
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; init; }
    [JsonPropertyName("httpBody")]
    public string? Body { get; init; }
    [JsonPropertyName("headers")] 
    public Dictionary<string, string> Headers { get; init; }
}