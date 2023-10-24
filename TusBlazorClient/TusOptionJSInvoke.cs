using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusOptionJsInvoke
{
    
    private readonly TusOptions _tusOptions;

    internal TusOptionJsInvoke(TusOptions tusOptions)
    {
        _tusOptions = tusOptions;
    }
    
    [JSInvokable]
    public void InvokeOnProgress(long bytesSent, long bytesTotal)
    {
        _tusOptions.OnProgress?.Invoke(bytesSent, bytesTotal);
    }
    
    [JSInvokable]
    public void InvokeOnChunkComplete(long chunkSize ,long bytesAccepted, long bytesTotal)
    {
        _tusOptions.OnChunkComplete?.Invoke(chunkSize, bytesAccepted, bytesTotal);
    }

    [JSInvokable]
    public void InvokeOnSuccess()
    {
        _tusOptions.OnSuccess?.Invoke();
    }
    
    [JSInvokable]
    public void InvokeOnError(string errorMsg, TusHttpRequest request, TusHttpResponse response)
    {
        var req = !string.IsNullOrEmpty(request.Method) ? request : null;
        var res = response.StatusCode <= 0 ? response : null;
        _tusOptions.OnError?.Invoke(new TusError(errorMsg, req, res));
    }

    [JSInvokable]
    public bool InvokeOnShouldRetry(string errorMsg, TusHttpRequest request, TusHttpResponse response, long retryAttempt)
    {
        var req = !string.IsNullOrEmpty(request.Method) ? request : null;
        var res = response.StatusCode <= 0 ? response : null;
        return _tusOptions.OnShouldRetry?.Invoke(new TusError(errorMsg, req, res), retryAttempt) ?? true;
    }
    
    [JSInvokable]
    public void InvokeOnBeforeRequest(TusHttpRequest request)
    {
        _tusOptions.OnBeforeRequest?.Invoke(request);
    }
    
    [JSInvokable]
    public void InvokeOnAfterResponse(TusHttpRequest request, TusHttpResponse response)
    {
        _tusOptions.OnAfterResponse?.Invoke(request, response);
    }
}