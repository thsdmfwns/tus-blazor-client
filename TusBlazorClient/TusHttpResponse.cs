namespace TusBlazorClient;

public class TusHttpResponse
{
    public int StatusCode { get; set; } = 0;
    public string? Body { get; set; }
    public string GetHeader(string key)
    {
        //todo GetHeader
        throw new MissingMethodException();
    }
}