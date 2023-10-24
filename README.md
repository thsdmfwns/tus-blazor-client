# tus-blazor-client
<img alt="Tus logo" src="https://github.com/tus/tus.io/blob/main/public/images/tus1.png?raw=true" width="30%" align="right" />

> **tus** is a protocol based on HTTP for _resumable file uploads_. Resumable
> means that an upload can be interrupted at any moment and can be resumed without
> re-uploading the previous data again. An interruption may happen willingly, if
> the user wants to pause, or by accident in case of an network issue or server
> outage.

tus-blazor-client is a wrapper library project for [tus-js-client](https://github.com/tus/tus-js-client) that can be used in .NET Blazor.


## Why do I use this?
The file I/O speed in .NET Blazor is not suitable for sending large files, and there are also limitations on the size of files that can be transferred. Therefore, sending large files with pure C# code in Blazor can be a challenging task.

## Installation

### Install tus-js-client

- Unminified version: [tus.js](https://cdn.jsdelivr.net/npm/tus-js-client@latest/dist/tus.js)
- Minified version: [tus.min.js](https://cdn.jsdelivr.net/npm/tus-js-client@latest/dist/tus.min.js) (recommended)

```html
<!-- project/wwwroot/index.html -->
...
    <div id="blazor-error-ui">
        An unhandled error has occurred.
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>
    <script src="_framework/blazor.webassembly.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/tus-js-client@latest/dist/tus.min.js"></script>
```



### Install tus-blazor-client
use [Nuget](https://www.nuget.org/packages/TusBlazorClient/) ```dotnet add package TusBlazorClient --version 1.0.1```

### Add TusBlazorClient in ServiceProvider
```csharp
// project/Program.cs
...
builder.Services.AddTusBlazorClient();
await builder.Build().RunAsync();
```

## Example
````csharp
@inject TusClient TusClient

<input type="file" @ref="_fileElement"/>
<button onclick="@Upload">upload</button>

@code {
    private ElementReference _fileElement;
    private TusUpload? _tusUpload;
    
    private async Task Upload()
    {
        // Get the selected file from the input element
        var file =  (await TusClient.GetFileInputElement(_fileElement).GetFiles()).First();
        // Get the selected file's info
        var fileInfo = await file.GetFileInfo();
        // Create a new tus upload
        var opt = new TusOptions
        {
            Endpoint = new Uri("http://localhost:1080/files"),
            Metadata = new Dictionary<string, string>()
            {
                {"filename", fileInfo.Name},
            },
            OnError = (err) =>
            {
                Console.WriteLine($"Failed because: {err.ErrorMessage}");
            },
            OnProgress = (bytesUploaded, bytesTotal) =>
            {
                var percentage = (double)bytesUploaded / bytesTotal;
                Console.WriteLine($"{bytesUploaded} {bytesTotal} {percentage:F}%");
            },
            OnSuccess = async () =>
            {
                var url = await _tusUpload.GetUrl();
                Console.WriteLine($"Download {fileInfo.Name} from {url}");
            },
        };
        _tusUpload = await TusClient.Upload(file, opt);
        
        // Check if there are any previous uploads to continue.
        var previousUploads = await _tusUpload.FindPreviousUpload();
        if (previousUploads.Count > 0)
        {
            // Found previous uploads so we select the first one.
            await _tusUpload.ResumeFromPreviousUpload(previousUploads.First());
        }
        
        // Start the upload
        await _tusUpload.Start();
    }
}
````

### API
Check [Wiki](https://github.com/thsdmfwns/tus-blazor-client/wiki)

