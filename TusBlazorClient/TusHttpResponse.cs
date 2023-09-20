using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusHttpResponse
{
    private TusHttpResponse(IJSObjectReference jsObject, int statusCode, string body)
    {
        JsObject = jsObject;
        StatusCode = statusCode;
        Body = body;
    }

    public IJSObjectReference JsObject { get; }
    public int StatusCode { get; }
    public string Body { get; }
    public async Task<string> GetHeaderAsync(string key) => await JsObject.InvokeAsync<string>("getHeader", key);

    public static async Task<TusHttpResponse?> FromJsObjectAsync(IJSObjectReference? jsObjectReference)
    {
        if (jsObjectReference is null)
        {
            return null;
        }
        return new TusHttpResponse(jsObjectReference, 
            await jsObjectReference.InvokeAsync<int>("getStatus"),
            await jsObjectReference.InvokeAsync<string>("getBody"));
    }
}