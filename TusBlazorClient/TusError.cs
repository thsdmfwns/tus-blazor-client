namespace TusBlazorClient;

public class TusError
{
    public string ErrorMessage { get; set; }
    public TusHttpRequest OriginalHttpRequest { get; set; }
    public TusHttpResponse OriginalHttpRespnse { get; set; }
}



