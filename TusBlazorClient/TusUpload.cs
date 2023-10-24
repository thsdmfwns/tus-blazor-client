using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusUpload : IAsyncDisposable
{
    internal TusUpload(IJSObjectReference uploadUploadObjectReference, TusOptions options, DotNetObjectReference<TusOptionJsInvoke> optionJsInvokeReference, TusJsInterop tusJsInterop)
    {
        _uploadObjectReference = uploadUploadObjectReference;
        _options = options;
        _optionJsInvokeReference = optionJsInvokeReference;
        _tusJsInterop = tusJsInterop;
    }

    private readonly TusOptions _options;
    private readonly IJSObjectReference _uploadObjectReference;
    private readonly TusJsInterop _tusJsInterop;
    private DotNetObjectReference<TusOptionJsInvoke>? _optionJsInvokeReference;

    public async ValueTask<IJSObjectReference> GetFile() =>
        await _tusJsInterop.GetFileFromUpload(_uploadObjectReference);
    public async ValueTask<JsFileInfo> GetFileInfo()
    {
        await using var file = await GetFile();
        return await _tusJsInterop.GetFileInfo(file);
    }
    
    /// <summary>
    /// Get URL used to upload the file.
    /// The url will be set once an upload has been created,
    /// which happens at last when the onSuccess callback is invoked.
    /// To resume an upload from a specific URL use the uploadUrl option instead.
    /// </summary>
    public async ValueTask<string> GetUrl() =>
        await _tusJsInterop.GeUrlFromUpload(_uploadObjectReference);

    public async ValueTask<TusOptions> GetOptions()
    {
        var options = await _tusJsInterop.GetOptionFromUpload(_uploadObjectReference);
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
        _optionJsInvokeReference = DotNetObjectReference.Create(new TusOptionJsInvoke(options));
        await _tusJsInterop.SetTusUploadOption(_uploadObjectReference, options, _optionJsInvokeReference,
            new TusOptionNullCheck(options));
        
    }
    
    public async Task Start()
    {
        await _uploadObjectReference.InvokeVoidAsync("start");
    }

    public async Task Abort(bool shouldTerminate)
    {
        await _uploadObjectReference.InvokeVoidAsync("abort", shouldTerminate);
    }

    public async Task Terminate(Uri uri)
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
        await _tusJsInterop.ResumeFromPreviousUpload(_uploadObjectReference, previousUploadRef.Index);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _uploadObjectReference.DisposeAsync();
        _optionJsInvokeReference?.Dispose();
    }
}