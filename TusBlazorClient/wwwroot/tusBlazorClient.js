//DotNet.createJSObjectReference
//dotNetHelper.invokeMethodAsync
export function IsSupported() {
    return window.tus.isSupported;
}

export function CanStoreUrls() {
    return window.tus.canStoreURLs;
}

export function GetFileListLength(htmlElement){
    if (!(htmlElement instanceof HTMLInputElement)) {
        return -1;   
    }
    return htmlElement.files.length;
}

export function GetFile(htmlElement, index){
    if (!(htmlElement instanceof HTMLInputElement)) {
        return null;
    }
    return htmlElement.files[index];
}

export function GetUplaod(file, opt, dotnetObject) {
    console.log(JSON.stringify(opt));
    return new window.tus.Upload(file, {
        endpoint: opt.endpoint,
        retryDelays: opt.retryDelays,
        metadata: opt.metadata,
        headers: opt.headers,
        chunkSize: opt.chunkSize ?? Infinity,
        uploadUrl: opt.uploadUrl,
        storeFingerprintForResuming: opt.storeFingerprintForResuming,
        removeFingerprintOnSuccess: opt.removeFingerprintOnSuccess,
        uploadLengthDeferred: opt.uploadLengthDeferred,
        uploadDataDuringCreation: opt.uploadDataDuringCreation,
        addRequestId: opt.addRequestId,
        parallelUploads: opt.parallelUploads,
        parallelUploadBoundaries: opt.parallelUploadBoundaries,
        onError: function (err) {
            if (opt.isNullOnError) return;
            let req = err.originalRequest
                ? new HttpRequest(err.originalRequest.getMethod(), err.originalRequest.getURL()) 
                : new HttpRequest();
            let res = err.originalResponse
                ? new HttpResponse(
                    err.originalResponse.getStatus(),
                    err.originalResponse.getBody(),
                    parseHeader(err.originalResponse.getUnderlyingObject().getAllResponseHeaders()))
                : new HttpResponse();
            dotnetObject.invokeMethodAsync("InvokeOnError", err.toString(), req, res);
        },
        onProgress: function (bytesUploaded, bytesTotal) {
            if (opt.isNullOnProgress) return;
            dotnetObject.invokeMethodAsync("InvokeOnProgress", bytesUploaded, bytesTotal);
        },
        onChunkComplete: function (chunkSize, bytesUploaded, bytesTotal) {
            if (opt.isNullOnChunkComplete) return;
            dotnetObject.invokeMethodAsync("InvokeOnChunkComplete", chunkSize, bytesUploaded, bytesTotal);
        },
        onSuccess: function () {
            if (opt.isNullOnSuccess) return;
            dotnetObject.invokeMethodAsync("InvokeOnSuccess");
        },
        onBeforeRequest: function (ogReq) {
            if (opt.isNullOnBeforeRequest) return;
            let req = new HttpRequest(ogReq.getMethod(), ogReq.getURL());
            dotnetObject.invokeMethod("InvokeOnBeforeRequest", req);
        },
        onAfterResponse: function (ogReq, ogRes) {
            if (opt.isNullOnAfterResponse) return;
            let req = new HttpRequest(ogReq.getMethod(), ogReq.getURL());
            let res = new HttpResponse(ogRes.getStatus(), ogRes.getBody(), parseHeader(ogRes.getUnderlyingObject().getAllResponseHeaders()));
            dotnetObject.invokeMethod("InvokeOnAfterResponse", req, res);
        },
        onShouldRetry: function (err, retryAttempt, _) {
            if (opt.isNullOnShouldRetry) return;
            let req = err.originalRequest !== null
                ? new HttpRequest(err.originalRequest.getMethod(), err.originalRequest.getURL())
                : new HttpRequest();
            let res = err.originalResponse !== null
                ? new HttpResponse(
                    err.originalResponse.getStatus(),
                    err.originalResponse.getBody(),
                    parseHeader(err.originalResponse.getUnderlyingObject().getAllResponseHeaders()))
                : new HttpResponse();
            return dotnetObject.invokeMethod("InvokeOnShouldRetry", err.toString(), req, res, retryAttempt);
        }
    });
}

function parseHeader(headerText){
    const arr = headerText.trim().split(/[\r\n]+/);
    const headerMap = {};
    
    arr.forEach((line) => {
        const parts = line.split(": ");
        const header = parts.shift();
        const value = parts.join(": ");
        headerMap[header] = value;
    });
    
    return headerMap;
}

class HttpRequest {
    constructor(method, url) {
        this.method = method ?? "";
        this.url = url ?? "";
    }
}

class HttpResponse {
    constructor(statusCode, httpBody, headers) {
        this.statusCode = statusCode ?? -1;
        this.httpbody = httpBody;
        this.headers = headers ?? {};
    }
}
