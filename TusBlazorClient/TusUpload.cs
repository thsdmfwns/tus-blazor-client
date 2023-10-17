using System.Text.Json;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusUpload : IAsyncDisposable
{
    private TusUpload(IJSObjectReference uploadJsObjectReference, TusOptions options, IJSObjectReference script, DotNetObjectReference<TusOptionJsInvoke> optionJsInvokereference)
    {
        _jsObjectReference = uploadJsObjectReference;
        _options = options;
        _script = script;
        _optionJsInvokeReference = optionJsInvokereference;
    }

    private readonly TusOptions _options;
    private readonly IJSObjectReference _jsObjectReference;
    private readonly IJSObjectReference _script;
    private readonly DotNetObjectReference<TusOptionJsInvoke> _optionJsInvokeReference;

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
        _optionJsInvokeReference.Dispose();
    }
}