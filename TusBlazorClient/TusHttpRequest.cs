using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusHttpRequest
{
    private TusHttpRequest(IJSObjectReference jsObject, string url, string method)
    {
        JsObject = jsObject;
        Url = url;
        Method = method;
    }

    public IJSObjectReference JsObject { get; }
    public string Method { get;}
    public string Url { get;}
    public async Task<string> GetHeaderAsync(string key) => await JsObject.InvokeAsync<string>("getHeader", key);
    public async Task SetHeaderAsync(string key, string value) => await JsObject.InvokeVoidAsync("setHeader", key, value);
    
    public static async Task<TusHttpRequest?> FromJsObjectAsync(IJSObjectReference? jsObjectReference)
    {
        if (jsObjectReference is null)
        {
            return null;
        }
        return new TusHttpRequest(jsObjectReference, 
            await jsObjectReference.InvokeAsync<string>("getURL"),
            await jsObjectReference.InvokeAsync<string>("getMethod"));
    }
}