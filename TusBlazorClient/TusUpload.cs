using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;

namespace TusBlazorClient;

public class TusUpload
{
    public TusUpload(IJSObjectReference fIleObject, TusOptions options)
    {
        FIleObject = fIleObject;
        Options = options;
        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

    private readonly DotNetObjectReference<TusUpload> _dotNetObjectReference;
    public TusOptions Options { get; }
    public IJSObjectReference FIleObject { get; }

    public async Task Start()
    {
        //todo set method
        throw new MissingMethodException();
    }

    public async Task Abort(bool shouldTerminate)
    {
        //todo set method
        throw new MissingMethodException();
    }

    public async Task Terminate(Uri uri, TusOptions? options)
    {
        //todo set method
        throw new MissingMethodException();
    }

    public async Task<List<TusPreviousUpload>> FindPreviousUpload()
    {
        //todo set method
        throw new MissingMethodException();
    }

    public async Task ResumeFromPreviousUpload(TusPreviousUpload previousUpload)
    {
        //todo set method
        throw new MissingMethodException();
    }

    [JSInvokable]
    public void OnProgress(long bytesSent, long bytesTotal)
    {
        Options.OnProgress?.Invoke(bytesSent, bytesTotal);
    }
    
    [JSInvokable]
    public void OnChunkComplete(long chunkSize ,long bytesAccepted, long bytesTotal)
    {
        Options.OnChunkComplete?.Invoke(chunkSize, bytesAccepted, bytesTotal);
    }

    [JSInvokable]
    public void OnSuccess()
    {
        Options.OnSuccess?.Invoke();
    }
    
    [JSInvokable]
    public async Task OnError(string errorMsg, IJSObjectReference? requestObject, IJSObjectReference? responseObject)
    {
        var req = await TusHttpRequest.FromJsObjectAsync(requestObject);
        var res = await TusHttpResponse.FromJsObjectAsync(requestObject);
        Options.OnError?.Invoke(new TusError(errorMsg, req, res));
    }

    [JSInvokable]
    public async Task<bool> OnError(string errorMsg, IJSObjectReference? requestObject,
        IJSObjectReference? responseObject, long retryAttempt, TusOptions tusOptions)
    {
        var req = await TusHttpRequest.FromJsObjectAsync(requestObject);
        var res = await TusHttpResponse.FromJsObjectAsync(requestObject);
        return Options.OnShouldRetry?.Invoke(new TusError(errorMsg, req, res), retryAttempt, tusOptions) ?? true;
    }
    
    [JSInvokable]
    public async Task OnBeforeRequest(IJSObjectReference requestObject)
    {
        var req = await TusHttpRequest.FromJsObjectAsync(requestObject);
        if (req is null)
        {
            return;
        }
        Options.OnBeforeRequest?.Invoke(req);
    }
    
    [JSInvokable]
    public async Task OnAfterResponse(IJSObjectReference requestObject, IJSObjectReference responseObject)
    {
        var req = await TusHttpRequest.FromJsObjectAsync(requestObject);
        var res = await TusHttpResponse.FromJsObjectAsync(responseObject);
        if (req is null || res is null)
        {
            return;
        }
        Options.OnAfterResponse?.Invoke(req, res);
    }
}