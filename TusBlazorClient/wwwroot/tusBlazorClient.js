//DotNet.createJSObjectReference
//dotNetHelper.invokeMethodAsync

const assembly = "TusBlazorClient";

export function IsSupported() {
    return window.tus.isSupported;
}

export function CanStoreUrls() {
    return window.tus.canStoreURLs;
}

export function GetFileListLength(htmlElement){
    if (!(htmlElement instanceof HTMLInputElement)) {
        console.log(`${assembly} : The html element is not file input element.`)
        return -1;   
    }
    return htmlElement.files.length;
}

export function GetFile(jsFileRef){
    return jsFileRef.elementReference.files[jsFileRef.index];
}

export function GetFileInfoFromJsFileRef(jsFileRef){
    const file = jsFileRef.elementReference.files[jsFileRef.index];
    return new FileInfo(file.name, file.size, file.lastModified);
}

export function GetFileInfo(file){
    return new FileInfo(file.name, file.size, file.lastModified);
}

export function GetUploadByJsFileRef (jsFileRef, opt, dotnetObject, optNullCheck) {
    const file = jsFileRef.elementReference.files[jsFileRef.index];
    return GetUpload(file, opt, dotnetObject, optNullCheck);
}

export function GetUpload(file, opt, dotnetObject, optNullCheck) {
    return new window.tus.Upload(file, GetUploadOption(opt, dotnetObject, optNullCheck));
}

export async function resumeFromPreviousUpload(tusUpload, index) {
    let pres = await tusUpload.findPreviousUploads();
    tusUpload.resumeFromPreviousUpload(pres[index]);
}

export function GetFileFromUpload(tusUpload) {
    return tusUpload.file;
}

export function GeUrlFromUpload(tusUpload) {
    return tusUpload.url;
}

export function GetOptionFromUpload(tusUpload) {
    let opt = tusUpload.options;
    console.log(opt);
    return {
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
        parallelUploadBoundaries: opt.parallelUploadBoundaries
    }
}

export function SetTusUploadOption(tusUpload, options, dotnetObj, optNullCheck) {
    let opt = GetUploadOption(options, dotnetObj, optNullCheck);
    let uploadOpt = tusUpload.options;
    uploadOpt.endpoint = opt.endpoint;
    uploadOpt.retryDelays = opt.retryDelays;
    uploadOpt.metadata = opt.metadata;
    uploadOpt.headers = opt.headers;
    uploadOpt.chunkSize = opt.chunkSize ?? Infinity;
    uploadOpt.uploadUrl = opt.uploadUrl;
    uploadOpt.storeFingerprintForResuming = opt.storeFingerprintForResuming;
    uploadOpt.removeFingerprintOnSuccess = opt.removeFingerprintOnSuccess;
    uploadOpt.uploadLengthDeferred = opt.uploadLengthDeferred;
    uploadOpt.uploadDataDuringCreation = opt.uploadDataDuringCreation;
    uploadOpt.addRequestId = opt.addRequestId;
    uploadOpt.parallelUploads = opt.parallelUploads;
    uploadOpt.parallelUploadBoundaries = opt.parallelUploadBoundaries;
    uploadOpt.onError = function (err) {
        if (optNullCheck.isNullOnError) return;
        let req = err.originalRequest
            ? new HttpRequest(err.originalRequest.getMethod(), err.originalRequest.getURL())
            : new HttpRequest();
        let res = err.originalResponse
            ? new HttpResponse(
                err.originalResponse.getStatus(),
                err.originalResponse.getBody(),
                parseHeader(err.originalResponse.getUnderlyingObject().getAllResponseHeaders()))
            : new HttpResponse();
        dotnetObj.invokeMethodAsync("InvokeOnError", err.toString(), req, res);
    };
    uploadOpt.onProgress = function (bytesUploaded, bytesTotal) {
        if (optNullCheck.isNullOnProgress) return;
        dotnetObj.invokeMethodAsync("InvokeOnProgress", bytesUploaded, bytesTotal);
    };
    uploadOpt.onChunkComplete = function (chunkSize, bytesUploaded, bytesTotal) {
        if (optNullCheck.isNullOnChunkComplete) return;
        dotnetObj.invokeMethodAsync("InvokeOnChunkComplete", chunkSize, bytesUploaded, bytesTotal);
    };
    uploadOpt.onSuccess = function () {
        if (optNullCheck.isNullOnSuccess) return;
        dotnetObj.invokeMethodAsync("InvokeOnSuccess");
    };
    uploadOpt.onBeforeRequest = function (ogReq) {
        if (optNullCheck.isNullOnBeforeRequest) return;
        let req = new HttpRequest(ogReq.getMethod(), ogReq.getURL());
        dotnetObj.invokeMethod("InvokeOnBeforeRequest", req);
    };
    uploadOpt.onAfterResponse = function (ogReq, ogRes) {
        if (optNullCheck.isNullOnAfterResponse) return;
        let req = new HttpRequest(ogReq.getMethod(), ogReq.getURL());
        let res = new HttpResponse(ogRes.getStatus(), ogRes.getBody(), parseHeader(ogRes.getUnderlyingObject().getAllResponseHeaders()));
        dotnetObj.invokeMethod("InvokeOnAfterResponse", req, res);
    };
    uploadOpt.onShouldRetry = function (err, retryAttempt, _) {
        if (optNullCheck.isNullOnShouldRetry) return;
        let req = err.originalRequest !== null
            ? new HttpRequest(err.originalRequest.getMethod(), err.originalRequest.getURL())
            : new HttpRequest();
        let res = err.originalResponse !== null
            ? new HttpResponse(
                err.originalResponse.getStatus(),
                err.originalResponse.getBody(),
                parseHeader(err.originalResponse.getUnderlyingObject().getAllResponseHeaders()))
            : new HttpResponse();
        return dotnetObj.invokeMethod("InvokeOnShouldRetry", err.toString(), req, res, retryAttempt);
    };
}

function GetUploadOption(opt, dotnetObject, optNullCheck){
    return {
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
        if (optNullCheck.isNullOnError) return;
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
            if (optNullCheck.isNullOnProgress) return;
            dotnetObject.invokeMethodAsync("InvokeOnProgress", bytesUploaded, bytesTotal);
        },
        onChunkComplete: function (chunkSize, bytesUploaded, bytesTotal) {
            if (optNullCheck.isNullOnChunkComplete) return;
            dotnetObject.invokeMethodAsync("InvokeOnChunkComplete", chunkSize, bytesUploaded, bytesTotal);
        },
        onSuccess: function () {
            if (optNullCheck.isNullOnSuccess) return;
            dotnetObject.invokeMethodAsync("InvokeOnSuccess");
        },
        onBeforeRequest: function (ogReq) {
            if (optNullCheck.isNullOnBeforeRequest) return;
            let req = new HttpRequest(ogReq.getMethod(), ogReq.getURL());
            dotnetObject.invokeMethod("InvokeOnBeforeRequest", req);
        },
        onAfterResponse: function (ogReq, ogRes) {
            if (optNullCheck.isNullOnAfterResponse) return;
            let req = new HttpRequest(ogReq.getMethod(), ogReq.getURL());
            let res = new HttpResponse(ogRes.getStatus(), ogRes.getBody(), parseHeader(ogRes.getUnderlyingObject().getAllResponseHeaders()));
            dotnetObject.invokeMethod("InvokeOnAfterResponse", req, res);
        },
        onShouldRetry: function (err, retryAttempt, _) {
            if (optNullCheck.isNullOnShouldRetry) return;
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
    }
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

class FileInfo {
    constructor(name, size, lastModified) {
        this.name = name;
        this.size = size ?? 0;
        this.lastModified = lastModified ?? "Unknown";
    }
}