using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;

namespace TusBlazorClient;

public class TusClient : IAsyncDisposable
{
    private readonly TusJsInterop _tusJsInterop;

    public TusClient(IJSRuntime jsRuntime)
    {
        _tusJsInterop = new TusJsInterop(jsRuntime);
    }


    public async Task<TusUpload> Upload(IJSObjectReference fileObjectRef, TusOptions options)
    {
        var jsInvokeReference = DotNetObjectReference.Create(new TusOptionJsInvoke(options));
        var uploadRef = await _tusJsInterop.GetUpload(fileObjectRef, options, jsInvokeReference, new TusOptionNullCheck(options));
        return new TusUpload(uploadRef, options, jsInvokeReference, _tusJsInterop);
    }
    
    public async Task<TusUpload> Upload(JsFile file, TusOptions options)
    {
        var jsInvokeReference = DotNetObjectReference.Create(new TusOptionJsInvoke(options));
        var uploadRef = await _tusJsInterop.GetUploadByJsFileRef(file, options, jsInvokeReference, new TusOptionNullCheck(options));
        return new TusUpload(uploadRef, options, jsInvokeReference, _tusJsInterop);
    }
    
    
    public async Task<bool> IsSupported()
    {
        try
        {
            return await _tusJsInterop.IsSupported();
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CanStoreUrls()
    {
        try
        {
            return await _tusJsInterop.CanStoreUrls();
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public FileInputElement GetFileInputElement(ElementReference htmlElement)
    {
        return new FileInputElement(htmlElement, _tusJsInterop);
    }

    public async ValueTask DisposeAsync()
    {
        await _tusJsInterop.DisposeAsync();
    }
}