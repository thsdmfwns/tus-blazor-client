namespace TusBlazorClient;

public class TusHttpRequest
{
    public string Method { get; set; }
    public string Url { get; set; }

    public string GetHeader(string key)
    {
        //todo GetHeader
        throw new MissingMethodException();
    }
}