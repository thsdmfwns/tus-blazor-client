using System.Text.Json;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusUpload : IAsyncDisposable
{
    private TusUpload(IJSObjectReference uploadJsObjectReference, TusOptions options, IJSObjectReference script)
    {
        _jsObjectReference = uploadJsObjectReference;
        _options = options;
        _script = script;
    }

    private readonly TusOptions _options;
    private readonly IJSObjectReference _jsObjectReference;
    private readonly IJSObjectReference _script;

    internal static async ValueTask<TusUpload> Create(IJSObjectReference script, TusOptions options, IJSObjectReference fileJsObject)
    {
        var optObject = DotNetObjectReference.Create(options);
        var uploadRef = await script.InvokeAsync<IJSObjectReference>("GetUpload", fileJsObject, options, optObject);
        return new TusUpload(uploadRef, options, script);
    }
    
    internal static async ValueTask<TusUpload> Create(IJSObjectReference script, TusOptions options, JsFileRef fileRef)
    {
        var optObject = DotNetObjectReference.Create(options);
        var uploadRef = await script.InvokeAsync<IJSObjectReference>("GetUploadByJsFileRef", fileRef, options, optObject);
        return new TusUpload(uploadRef, options, script);
    }
    
    public async Task Start()
    {
        await _jsObjectReference.InvokeVoidAsync("start");
    }

    public async Task Abort(bool shouldTerminate)
    {
        await _jsObjectReference.InvokeVoidAsync("abort", shouldTerminate);
    }

    public async Task Terminate(Uri uri, TusOptions? options)
    {
        await _jsObjectReference.InvokeVoidAsync("terminate", uri.ToString());
    }

    public async ValueTask<List<TusPreviousUploadRef>> FindPreviousUpload()
    {
        var list = await _jsObjectReference.InvokeAsync<List<TusPreviousUpload>>("findPreviousUploads");
        return Enumerable.Range(0, list.Count)
            .Select(x => new TusPreviousUploadRef(x, list.ElementAt(x))).ToList();
    }

    public async Task ResumeFromPreviousUpload(TusPreviousUploadRef previousUploadRef)
    {
        await _script.InvokeVoidAsync("resumeFromPreviousUpload", _jsObjectReference, previousUploadRef.Index);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _jsObjectReference.DisposeAsync();
    }
}