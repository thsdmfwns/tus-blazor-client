namespace TusBlazorClient;

public class TusOptionNullCheck
{
    private readonly TusOptions _tusOptions;

    public TusOptionNullCheck(TusOptions tusOptions)
    {
        _tusOptions = tusOptions;
    }
    
    public bool IsNullOnProgress => _tusOptions.OnProgress is null;
    public bool IsNullOnChunkComplete => _tusOptions.OnChunkComplete is null;
    public bool IsNullOnSuccess => _tusOptions.OnSuccess is null;
    public bool IsNullOnError => _tusOptions.OnError is null;
    public bool IsNullOnShouldRetry => _tusOptions.OnShouldRetry is null;
    public bool IsNullOnBeforeRequest => _tusOptions.OnBeforeRequest is null;
    public bool IsNullOnAfterResponse => _tusOptions.OnAfterResponse is null;
    
}