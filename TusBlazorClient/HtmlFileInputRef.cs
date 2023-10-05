using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class HtmlFileInputRef
{
    private readonly ElementReference _element;
    private readonly IJSObjectReference _script;

    internal HtmlFileInputRef(ElementReference inputElementReference, IJSObjectReference scripRef)
    {
        _element = inputElementReference;
        _script = scripRef;
    }

    public async ValueTask<int> Length()
    {
        return await _script.InvokeAsync<int>("GetFileListLength", _element);
    }

    public async ValueTask<List<JsFileRef>> GetFiles()
    {
        return Enumerable.Range(0, Math.Max(0, await Length()))
            .Select(index => new JsFileRef(this, index)).ToList();
    }

    internal async ValueTask<IJSObjectReference> AtElement(int index)
    {
        return await _script.InvokeAsync<IJSObjectReference>("GetFile", _element, index);
    }
}

public class JsFileRef
{
    private readonly HtmlFileInputRef _fileInput;
    private readonly int _index;

    public JsFileRef(HtmlFileInputRef fileInput, int index)
    {
        _fileInput = fileInput;
        _index = index;
    }

    public async ValueTask<IJSObjectReference> ToJsObjectRef() => await _fileInput.AtElement(_index);
}