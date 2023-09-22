using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusClient : IAsyncDisposable
{
    private readonly IJSObjectReference _script;
    public TusClient(IJSObjectReference jsObjectReference)
    {
        _script = jsObjectReference;
    }

    public static async Task<TusClient> Create(IJSRuntime jsRuntime)
    {
        return new TusClient(await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/TusBlazorClient/tusBlazorClient.js"));
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

    public async ValueTask DisposeAsync()
    {
        await _script.DisposeAsync();
    }
}