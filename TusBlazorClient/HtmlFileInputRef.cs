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

    internal async ValueTask<IJSObjectReference> AtElement(int index)
    {
        return await _script.InvokeAsync<IJSObjectReference>("GetFile", Element, index);
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

    public async ValueTask<IJSObjectReference> ToJsObjectRef() => await _fileInput.AtElement(Index);
}