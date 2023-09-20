namespace TusBlazorClient;

public class TusHttpRequest
{
    public string Method { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public string GetHeader(string key)
    {
        //todo GetHeader
        throw new MissingMethodException();
    }
}