using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class HtmlFileInputRef
{
    public ElementReference Element { get; init; }
    private readonly IJSObjectReference _script;

    internal HtmlFileInputRef(ElementReference inputElementReference, IJSObjectReference scripRef)
    {
        Element = inputElementReference;
        _script = scripRef;
    }

    public async ValueTask<int> Length()
    {
        return await _script.InvokeAsync<int>("GetFileListLength", Element);
    }

    public async ValueTask<List<JsFileRef>> GetFiles()
    {
        return Enumerable.Range(0, Math.Max(0, await Length()))
            .Select(index => new JsFileRef(this, index)).ToList();
    }

    internal async ValueTask<IJSObjectReference> GetFile(JsFileRef jsFileRef)
    {
        return await _script.InvokeAsync<IJSObjectReference>("GetFile", jsFileRef);
    }
    
    internal async ValueTask<JsFileInfo> GetFileInfo(JsFileRef jsFileRef)
    {
        return await _script.InvokeAsync<JsFileInfo>("GetFileInfoFromJsFileRef", jsFileRef);
    }
}

public class JsFileRef
{
    private readonly HtmlFileInputRef _fileInput;

    public int Index { get; init; } 
    public ElementReference ElementReference { get; init; } 
    public JsFileRef(HtmlFileInputRef fileInput, int index)
    {
        _fileInput = fileInput;
        ElementReference = fileInput.Element;
        Index = index;
    }

    public async ValueTask<IJSObjectReference> ToJsObjectRef() => await _fileInput.GetFile(this);
    public async ValueTask<JsFileInfo> GetFileInfo() => await _fileInput.GetFileInfo(this);
}

public class JsFileInfo
{
    public string Name { get; init; } = "";
    public long Size { get; init; } = 0;
    [JsonPropertyName("lastModified")] public long LastModifiedTicks { get; init; } = 0;
    [JsonIgnore] public DateTimeOffset LastModified => DateTimeOffset.FromUnixTimeMilliseconds(LastModifiedTicks).ToLocalTime();
}