using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class FileInputElement
{
    public ElementReference ElementReference { get; init; }
    private readonly TusJsInterop _tusJsInterop;

    internal FileInputElement(ElementReference inputElementReference, TusJsInterop tusJsInterop)
    {
        ElementReference = inputElementReference;
        _tusJsInterop = tusJsInterop;
    }

    public async ValueTask<int> Length()
    {
        return await _tusJsInterop.GetFileListLength(ElementReference);
    }

    public async ValueTask<List<JsFile>> GetFiles()
    {
        return Enumerable.Range(0, Math.Max(0, await Length()))
            .Select(index => new JsFile(this, index)).ToList();
    }

    internal async ValueTask<IJSObjectReference> GetFile(JsFile jsFile)
    {
        return await _tusJsInterop.GetFile(jsFile);
    }
    
    internal async ValueTask<JsFileInfo> GetFileInfo(JsFile jsFile)
    {
        return await _tusJsInterop.GetFileInfoFromJsFileRef(jsFile);
    }
}

public class JsFile
{
    private readonly FileInputElement _fileInput;
    public ElementReference ElementReference => _fileInput.ElementReference;
    public int Index { get; init; } 
    internal JsFile(FileInputElement fileInput, int index)
    {
        _fileInput = fileInput;
        Index = index;
    }

    public async ValueTask<IJSObjectReference> ToJsObjectReference() => await _fileInput.GetFile(this);
    public async ValueTask<JsFileInfo> GetFileInfo() => await _fileInput.GetFileInfo(this);
}

public class JsFileInfo
{
    public string Name { get; init; } = "";
    public long Size { get; init; } = 0;
    [JsonPropertyName("lastModified")] public long LastModifiedTicks { get; init; } = 0;
    [JsonIgnore] public DateTimeOffset LastModified => DateTimeOffset.FromUnixTimeMilliseconds(LastModifiedTicks).ToLocalTime();
}