using Microsoft.JSInterop;

namespace TusBlazorClient;

internal static class Utils
{
    internal static async Task<IJSObjectReference> GetScript(IJSRuntime jsRuntime)
    {
        return await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/TusBlazorClient/tusBlazorClient.js");
    }
}