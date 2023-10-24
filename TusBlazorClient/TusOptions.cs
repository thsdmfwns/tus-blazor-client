using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace TusBlazorClient;

public class TusOptions
{
    /// <summary>
    /// The upload creation URL which will be used to create new uploads.
    /// </summary>
    public Uri Endpoint { get; set; }

    /// <summary>
    /// A number indicating the maximum size of a PATCH request body in bytes. The default value (Infinity) means that tus-js-client will try to upload the entire file in one request. This setting is also required if the input file is a reader/readable stream.
    /// </summary>
    public long? ChunkSize { get; set; } = null;
    
    /// <summary>
    /// An optional function that will be called each time progress information is available. The arguments will be bytesSent and bytesTotal.
    /// <typeparam name="Left">BytesSent</typeparam>
    /// <typeparam name="Right">BytesTotal</typeparam>
    /// </summary>
    [JsonIgnore]
    public Action<long, long>? OnProgress { get; set; }
    
    
    /// <summary>
    /// An optional function that will be called each time a PATCH has been successfully completed. The arguments will be chunkSize, bytesAccepted, bytesTotal.
    /// <typeparam name="Left">ChunkSize</typeparam>
    /// <typeparam name="Middle">BytesAccepted</typeparam>
    /// <typeparam name="Right">BytesTotal</typeparam>
    /// </summary>
    [JsonIgnore]
    public Action<long, long, long>? OnChunkComplete { get; set; }
    
    /// <summary>
    /// An optional function called when the upload finished successfully.
    /// </summary>
    [JsonIgnore]
    public Action? OnSuccess { get; set; }

    
    /// <summary>
    /// An optional function called once an error appears. The argument will be an Error instance with additional information about the involved requests.
    /// <typeparam name="TusError">Error</typeparam>
    /// </summary>
    [JsonIgnore]
    public Action<TusError>? OnError { get; set; }

    /// <summary>
    /// An optional function called once an error appears and before retrying.
    /// <para>When no callback is specified, the retry behavior will be the default one:
    /// any status codes of 409, 423 or any other than 4XX will be treated as a server error and the request will be retried automatically,
    /// as long as the browser does not indicate that we are offline.</para>
    /// <para>When a callback is specified, its return value will influence the retry behavior:
    /// The function must return true if the request should be retried, false otherwise.
    /// The argument will be an Error instance with additional information about the involved requests.</para>
    /// <typeparam name="TusError">Error</typeparam>
    /// <typeparam name="Long">RetryAttempt</typeparam>
    /// <typeparam name="bool">Return value</typeparam>
    /// </summary>
    [JsonIgnore]
    public Func<TusError, long, bool>? OnShouldRetry { get; set; }

    /// <summary>
    /// An object with custom header values used in all requests. Useful for adding authentication details.
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    
    /// <summary>
    /// An object with string values used as additional meta data which will be passed along to the server when (and only when) creating a new upload. 
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// A URL which will be used to directly attempt a resume without creating an upload first.
    /// </summary>
    public Uri? UploadUrl { get; set; }

    /// <summary>
    /// An array or empty, indicating how many milliseconds should pass before the next attempt to uploading will be started after the transfer has been interrupted.
    /// <para>Default value: [0, 1000, 3000, 5000]</para>
    /// </summary>
    public List<int> RetryDelays { get; set; } = new List<int>() { 0, 1000, 3000, 5000 };

    /// <summary>
    /// Default value: true
    /// <para>A boolean indicating if the upload URL should be stored in the URL storage using the file's fingerprint after an new upload resource on the server has been created or an upload URL has been provided using the uploadUrl option. </para>
    /// <para> If enabled, the upload URL can later be retrieved from the URL storage using the tus.Upload#findPreviousUploads method.</para>
    /// </summary>
    public bool StoreFingerprintForResuming { get; set; } = true;
    
    /// <summary>
    /// Default value: false
    /// <para>A boolean indicating if the fingerprint in the URL storage will be removed once the upload is successfully completed.</para>
    /// </summary>
    public bool RemoveFingerprintOnSuccess { get; set; } = false;
    
    /// <summary>
    /// Default value: false
    /// <para>
    /// A boolean indicating whether a stream of data is going to be uploaded as a Reader. If so, the total size isn't available when we begin uploading, so we use the Tus Upload-Defer-Length header. 
    /// </para>
    /// </summary>
    public bool UploadLengthDeferred { get; set; } = false;
    
    /// <summary>
    /// Default value: false
    /// <para>
    /// A boolean indicating whether the creation-with-upload extension should be used. If true, the file's content will already be transferred in the POST request when a new upload is created. 
    /// </para>
    /// </summary>
    public bool UploadDataDuringCreation { get; set; } = false;
    
    /// <summary>
    /// Default value: false
    /// <para>
    /// A boolean indicating whether a random request ID should be added to every HTTP request that is sent. 
    /// </para>
    /// </summary>
    public bool AddRequestId { get; set; } = false;
    
    /// <summary>
    /// Default value: 1
    /// <para>
    /// A number indicating how many parts should be uploaded in parallel. If this number is not 1, the input file will be split into multiple parts, where each part is uploaded individually in parallel. 
    /// </para>
    /// </summary>
    public int ParallelUploads { get; set; } = 1;

    /// <summary>
    /// Default value: null
    /// <para>
    /// An array indicating the boundaries of the different parts uploaded during a parallel upload. This option is only considered if parallelUploads is greater than 1. 
    /// </para>
    /// </summary>
    public List<(int start, int end)>? ParallelUploadBoundaries { get; set; }
    
    /// <summary>
    /// An optional function that will be called before a HTTP request is sent out.
    /// </summary>
    [JsonIgnore]
    public Action<TusHttpRequest>? OnBeforeRequest { get; set; }
    
    /// <summary>
    /// An optional function that will be called after a HTTP response has been received.
    /// </summary>
    [JsonIgnore]
    public Action<TusHttpRequest?, TusHttpResponse?>? OnAfterResponse { get; set; }
}