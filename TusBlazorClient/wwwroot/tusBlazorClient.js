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

