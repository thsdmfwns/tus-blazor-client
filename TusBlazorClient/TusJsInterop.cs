using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusJsInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    
    private IJSObjectReference? _script;

    public TusJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task InitializeAsync()
    {
        if (_script is not null)
        {
            return;
        }
        _script = await _jsRuntime.InvokeAsync<IJSObjectReference>("import",
            "./_content/TusBlazorClient/tusBlazorClient.js");
    }

    public async ValueTask<bool> IsSupported()
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<bool>("IsSupported");
    }
    
    public async ValueTask<bool> CanStoreUrls()
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<bool>("CanStoreUrls");
    }

    public async ValueTask<IJSObjectReference> GetUpload(
        IJSObjectReference fileObjectRef,
        TusOptions options,
        DotNetObjectReference<TusOptionJsInvoke> jsInvokeReference,
        TusOptionNullCheck tusOptionNullCheck
    )
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<IJSObjectReference>("GetUpload", fileObjectRef, options, jsInvokeReference, tusOptionNullCheck);
    }

    public async ValueTask<IJSObjectReference> GetUploadByJsFileRef(
        JsFile file,
        TusOptions options,
        DotNetObjectReference<TusOptionJsInvoke> jsInvokeReference,
        TusOptionNullCheck tusOptionNullCheck
    )
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<IJSObjectReference>("GetUploadByJsFileRef", file, options, jsInvokeReference, tusOptionNullCheck);
    }

    public async ValueTask<int> GetFileListLength(ElementReference element)
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<int>("GetFileListLength", element);
    }
    
    internal async ValueTask<IJSObjectReference> GetFile(JsFile jsFile)
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<IJSObjectReference>("GetFile", jsFile);
    }
    
    internal async ValueTask<JsFileInfo> GetFileInfoFromJsFileRef(JsFile jsFile)
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<JsFileInfo>("GetFileInfoFromJsFileRef", jsFile);
    }

    public async ValueTask<IJSObjectReference> GetFileFromUpload(IJSObjectReference tusUploadJsObjectReference)
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<IJSObjectReference>("GetFileFromUpload", tusUploadJsObjectReference);
    }

    public async ValueTask<JsFileInfo> GetFileInfo(IJSObjectReference fileJsObjectReference)
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<JsFileInfo>("GetFileInfo", fileJsObjectReference);
    }

    public async ValueTask<string> GeUrlFromUpload(IJSObjectReference tusUploadJsObjectReference)
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<string>("GeUrlFromUpload", tusUploadJsObjectReference);
    }

    public async ValueTask<TusOptions> GetOptionFromUpload(IJSObjectReference tusUploadJsObjectReference)
    {
        await InitializeAsync();
        return await _script!.InvokeAsync<TusOptions>("GetOptionFromUpload", tusUploadJsObjectReference);
    }

    public async Task SetTusUploadOption(
        IJSObjectReference tusUploadJsObject,
        TusOptions tusOptions,
        DotNetObjectReference<TusOptionJsInvoke> tusOptionJsInvokeObject,
        TusOptionNullCheck tusOptionNullCheck)
    {
        await InitializeAsync();
        await _script!.InvokeVoidAsync("SetTusUploadOption",
            tusUploadJsObject, tusOptions,
            tusOptionJsInvokeObject, tusOptionNullCheck);
    }

    public async Task ResumeFromPreviousUpload(IJSObjectReference tusUploadJsObject, int index)
    {
        await InitializeAsync();
        await _script!.InvokeVoidAsync("resumeFromPreviousUpload", tusUploadJsObject, index);
    }

    public async ValueTask DisposeAsync()
    {
        if (_script != null)
        {
            await _script.DisposeAsync();
            _script = null;
        }
    }
}