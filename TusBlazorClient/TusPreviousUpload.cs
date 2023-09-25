namespace TusBlazorClient;

public class TusPreviousUpload
{
    public long? Size { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public string CreationTime { get; set; }
    public string UrlStorageKey { get; set; }
}