using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusClient
{
    private readonly IJSObjectReference _script;
    public TusClient(IJSRuntime jsRuntime)
    {
        _script = jsRuntime.InvokeAsync<IJSObjectReference>("import", "./tusBlazorClient.js").Result;
    }
    
    public static async Task<bool> IsSupported(IJSRuntime currentJsRuntime)
    {
        try
        {
            return await currentJsRuntime.InvokeAsync<bool>("tus.isSupported");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            return false;
        }
    }

    public static async Task<bool> CanStoreUrls(IJSRuntime currentJsRuntime)
    {
        try
        {
            return await currentJsRuntime.InvokeAsync<bool>("tus.canStoreURLs");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            return false;
        }
    }
}