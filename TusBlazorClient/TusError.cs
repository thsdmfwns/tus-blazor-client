namespace TusBlazorClient;

public class TusError
{
    public TusError(string errorMessage, TusHttpRequest? originalHttpRequest, TusHttpResponse? originalHttpResponse)
    {
        ErrorMessage = errorMessage;
        OriginalHttpRequest = originalHttpRequest;
        OriginalHttpResponse = originalHttpResponse;
    }

    public string ErrorMessage { get; }
    public TusHttpRequest? OriginalHttpRequest { get; }
    public TusHttpResponse? OriginalHttpResponse { get; }
}



