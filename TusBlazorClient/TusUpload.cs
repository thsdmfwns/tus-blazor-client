using System.Text.Json;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusUpload : IAsyncDisposable
{
    private TusUpload(IJSObjectReference uploadJsObjectReference, TusOptions options)
    {
        _jsObjectReference = uploadJsObjectReference;
        _options = options;
    }

    private readonly TusOptions _options;
    private readonly IJSObjectReference _jsObjectReference;

    public static async ValueTask<TusUpload> Create(IJSObjectReference script, TusOptions options, JsFileRef fileRef)
    {
        await using var file = await fileRef.ToJsObjectRef();
        var optObject = DotNetObjectReference.Create(options);
        var uploadRef = await script.InvokeAsync<IJSObjectReference>("GetUplaod", file, options, optObject);
        return new TusUpload(uploadRef, options);
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

    public async ValueTask<List<TusPreviousUpload>?> FindPreviousUpload()
    {
        return await _jsObjectReference.InvokeAsync<List<TusPreviousUpload>>("findPreviousUploads");
    }

    public async Task ResumeFromPreviousUpload(TusPreviousUpload previousUpload)
    {
        await _jsObjectReference.InvokeVoidAsync("resumeFromPreviousUpload", previousUpload);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _jsObjectReference.DisposeAsync();
    }
}