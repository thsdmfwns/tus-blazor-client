namespace TusBlazorClient;

public class TusError
{
    public string ErrorMessage { get; set; } = string.Empty;
    public TusHttpRequest? OriginalHttpRequest { get; set; }
    public TusHttpResponse? OriginalHttpResponse { get; set; }
}



