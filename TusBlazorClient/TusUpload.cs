using System.Text.Json;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusUpload : IAsyncDisposable
{
    private TusUpload(IJSObjectReference uploadUploadObjectReference, TusOptions options, IJSObjectReference script, DotNetObjectReference<TusOptionJsInvoke> optionJsInvokereference)
    {
        _uploadObjectReference = uploadUploadObjectReference;
        _options = options;
        _script = script;
        _optionJsInvokeReference = optionJsInvokereference;
    }

    private readonly TusOptions _options;
    private readonly IJSObjectReference _uploadObjectReference;
    private readonly IJSObjectReference _script;
    private DotNetObjectReference<TusOptionJsInvoke>? _optionJsInvokeReference;

    internal static async ValueTask<TusUpload> Create(IJSObjectReference script, TusOptions options, IJSObjectReference fileJsObject)
    {
        var optObject = DotNetObjectReference.Create(new TusOptionJsInvoke(options));
        var uploadRef = await script.InvokeAsync<IJSObjectReference>("GetUpload", fileJsObject, options, optObject, new TusOptionNullCheck(options));
        return new TusUpload(uploadRef, options, script, optObject);
    }
    
    internal static async ValueTask<TusUpload> Create(IJSObjectReference script, TusOptions options, JsFileRef fileRef)
    {
        var optObject = DotNetObjectReference.Create(new TusOptionJsInvoke(options));
        var uploadRef = await script.InvokeAsync<IJSObjectReference>("GetUploadByJsFileRef", fileRef, options, optObject, new TusOptionNullCheck(options));
        return new TusUpload(uploadRef, options, script, optObject);
    }

    public async ValueTask<TusOptions> GetOptions()
    {
        var options = await _script.InvokeAsync<TusOptions>("GetAliveTusUploadOption", _uploadObjectReference);
        //merge
        options.OnError = _options.OnError;
        options.OnProgress = _options.OnProgress;
        options.OnSuccess = _options.OnSuccess;
        options.OnAfterResponse = _options.OnAfterResponse;
        options.OnBeforeRequest = _options.OnBeforeRequest;
        options.OnChunkComplete = _options.OnChunkComplete;
        options.OnShouldRetry = _options.OnShouldRetry;
        return options;
    }

    public async Task SetOtions(Action<TusOptions> setOption)
    {
        var options = await GetOptions();
        setOption.Invoke(options);
        _optionJsInvokeReference?.Dispose();
        _optionJsInvokeReference = null;
        _optionJsInvokeReference = DotNetObjectReference.Create(new TusOptionJsInvoke(options));
        await _script.InvokeVoidAsync("SetTusUploadOption", _uploadObjectReference, options, _optionJsInvokeReference, new TusOptionNullCheck(options));
    }
    
    public async Task Start()
    {
        await _uploadObjectReference.InvokeVoidAsync("start");
    }

    public async Task Abort(bool shouldTerminate)
    {
        await _uploadObjectReference.InvokeVoidAsync("abort", shouldTerminate);
    }

    public async Task Terminate(Uri uri, TusOptions? options)
    {
        await _uploadObjectReference.InvokeVoidAsync("terminate", uri.ToString());
    }

    public async ValueTask<List<TusPreviousUploadRef>> FindPreviousUpload()
    {
        var list = await _uploadObjectReference.InvokeAsync<List<TusPreviousUpload>>("findPreviousUploads");
        return Enumerable.Range(0, list.Count)
            .Select(x => new TusPreviousUploadRef(x, list.ElementAt(x))).ToList();
    }

    public async Task ResumeFromPreviousUpload(TusPreviousUploadRef previousUploadRef)
    {
        await _script.InvokeVoidAsync("resumeFromPreviousUpload", _uploadObjectReference, previousUploadRef.Index);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _uploadObjectReference.DisposeAsync();
        _optionJsInvokeReference?.Dispose();
    }
}