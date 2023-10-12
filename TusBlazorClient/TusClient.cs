using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;

namespace TusBlazorClient;

public class TusClient : IAsyncDisposable
{
    private readonly IJSObjectReference _script;

    private TusClient(IJSObjectReference jsObjectReference)
    {
        _script = jsObjectReference;
    }

    public static async Task<TusClient> Create(IJSRuntime jsRuntime)
    {
        return new TusClient(await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/TusBlazorClient/tusBlazorClient.js"));
    }
    
    public async Task<TusUpload> Upload(JsFileRef fileRef, TusOptions options)
    {
        return await TusUpload.Create(_script, options, fileRef);
    }
    
    public async Task<bool> IsSupported()
    {
        try
        {
            return await _script.InvokeAsync<bool>("IsSupported");
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
            return await _script.InvokeAsync<bool>("CanStoreUrls");
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public HtmlFileInputRef GetHtmlFileInputRef(ElementReference htmlElement)
    {
        return new HtmlFileInputRef(htmlElement, _script);
    }

    public async ValueTask DisposeAsync()
    {
        await _script.DisposeAsync();
    }
}