using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TusBlazorClient.Test;

public class E2eTest : TestBase
{
    protected const string FilePath = "/home/son/test-image.png";

    [Test]
    public void Upload()
    {
        Init("http://localhost:5288/upload", FilePath);
        IWebElement uploadButton = _driver.FindElement(By.Id("upload-btn"));
        uploadButton.Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains(SuccessMsg));
        var finalOutput = _driver.FindElement(By.Id("output")).Text;
        var outputs = finalOutput.Split("\n");
        var urlStr = outputs.Single(x => x.StartsWith("===UploadUrl:")).Replace("===UploadUrl:", "");
        var fileInfoStr = outputs.Single(x => x.StartsWith("===uploadFile:")).Replace("===uploadFile:", "");
        var jsFileInfo = JsonSerializer.Deserialize<JsFileInfo>(fileInfoStr);
        var fileInfo = new FileInfo(FilePath);
        Assert.That(jsFileInfo, Is.Not.Null);
        Assert.That(jsFileInfo!.Name, Is.EqualTo(fileInfo.Name));
        Assert.That(Uri.TryCreate(urlStr, UriKind.RelativeOrAbsolute ,out var _), Is.True);
        Assert.That(finalOutput, Does.Contain(SuccessMsg));
        Assert.That(finalOutput, Does.Contain("===OnProgress"));
        Assert.That(finalOutput, Does.Contain("===OnChunkComplete"));
    }

    [Test]
    public void UploadByJsObj()
    {
        Init("http://localhost:5288/uploadByJsObj", FilePath);
        IWebElement uploadButton = _driver.FindElement(By.Id("upload-btn"));
        uploadButton.Click();
        var finalOutput = _driver.FindElement(By.Id("output")).Text;
        try
        {
            new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
                driver => driver.FindElement(By.Id("output")).Text.Contains(SuccessMsg));
        }
        finally
        {
            Assert.That(finalOutput, Does.Contain(SuccessMsg));
            Assert.That(finalOutput, Does.Contain("===OnProgress"));
            Assert.That(finalOutput, Does.Contain("===OnChunkComplete"));
        }
    }

    [Test]
    public void UploadResume()
    {
        Init("http://localhost:5288/uploadResume", FilePath,chunkSize: 50000);
        _driver.FindElement(By.Id("upload-btn")).Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains("====Stop"));
        Task.Delay(10).Wait();
        _driver.FindElement(By.Id("resume-btn")).Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains(SuccessMsg));
        var finalOutput = _driver.FindElement(By.Id("output")).Text;
        var firstProgressLog = finalOutput.Split("\n").SingleOrDefault(x => x.Contains("===firstProgress"));
        Assert.That(firstProgressLog, Is.Not.Null);
        var firstProgress = long.Parse(firstProgressLog!.Split(":")[1]);
        Assert.That(firstProgress, Is.Not.EqualTo(0));
        Assert.That(finalOutput, Does.Contain(SuccessMsg));
    }

    [Test]
    public void ResumeFromPreviousUpload()
    {
        Init("http://localhost:5288/ResumeFromPreviousUpload", FilePath, chunkSize: 50000);
        _driver.FindElement(By.Id("upload-btn")).Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains("====Stop"));
        Task.Delay(10).Wait();
        _driver.FindElement(By.Id("resume-btn")).Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains(SuccessMsg));
        var finalOutput = _driver.FindElement(By.Id("output")).Text;
        var firstProgressLog = finalOutput.Split("\n").SingleOrDefault(x => x.Contains("===firstProgress"));
        Assert.That(firstProgressLog, Is.Not.Null);
        var firstProgress = long.Parse(firstProgressLog!.Split(":")[1]);
        Assert.That(firstProgress, Is.Not.EqualTo(0));
        Assert.That(finalOutput, Does.Contain(SuccessMsg));
    }

    [Test]
    public void ShouldRetry()
    {
        Init("http://localhost:5288/shouldRetry", FilePath, isError: true);
        _driver.FindElement(By.Id("upload-btn")).Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains("===OnError"));
        var finalOutput = _driver.FindElement(By.Id("output")).Text;
        var outputs = finalOutput.Split("\n").ToList();
        var retryCount = outputs.Count(x => string.Equals(x, "===OnShouldRetry"));
        Assert.That(retryCount, Is.GreaterThanOrEqualTo(5));
        Assert.That(outputs, Does.Contain("===OnError"));
    }
    
    [Test]
    public void ShouldNotRetry()
    {
        Init("http://localhost:5288/shouldNoRetry", FilePath, isError: true);
        _driver.FindElement(By.Id("upload-btn")).Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains("===OnError"));
        var finalOutput = _driver.FindElement(By.Id("output")).Text;
        var outputs = finalOutput.Split("\n").ToList();
        var retryCount = outputs.Count(x => string.Equals(x, "===OnShouldRetry"));
        Assert.That(retryCount, Is.LessThanOrEqualTo(1));
        Assert.That(outputs, Does.Contain("===OnError"));
    }

    [Test]
    public void OnRequest()
    {
        Init("http://localhost:5288/OnRequest", FilePath);
        IWebElement uploadButton = _driver.FindElement(By.Id("upload-btn"));
        uploadButton.Click();
        var finalOutput = _driver.FindElement(By.Id("output")).Text;
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains(SuccessMsg));
        var outputs = finalOutput.Split("\n");
        var responseHeaders = outputs
            .Where(x => x.StartsWith("Response : "))
            .Select(x => JsonSerializer.Deserialize<TusHttpResponse>(x.Replace("Response : ", "")))
            .Where(x => x is not null)
            .Select(x => x!.Headers)
            .ToList();
        foreach (var header in responseHeaders)
        {
            Assert.That(header, Does.ContainKey("tus-resumable"));   
        }
        Assert.That(finalOutput, Does.Contain(SuccessMsg));
        Assert.That(finalOutput, Does.Contain("===OnAfterResponse"));
        Assert.That(finalOutput, Does.Contain("===OnBeforeRequest"));
    }

    [Test]
    public void SetOption()
    {
        //===ChunkSend:
        Init("http://localhost:5288/SetOption", FilePath, chunkSize: 50000);
        _driver.FindElement(By.Id("upload-btn")).Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains("====Stop"));
        Task.Delay(10).Wait();
        _driver.FindElement(By.Id("resume-btn")).Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains(SuccessMsg));
        var finalOutput = _driver.FindElement(By.Id("output")).Text;
        var outputs = finalOutput.Split("\n");
        var firstProgressLog = outputs.SingleOrDefault(x => x.StartsWith("===firstProgress"));
        Assert.That(firstProgressLog, Is.Not.Null);
        var firstProgress = long.Parse(firstProgressLog!.Split(":")[1]);
        var chunksizes = outputs.Where(x => x.Equals("===ChunkSend:15000")).ToList();
        Assert.That(firstProgress, Is.Not.EqualTo(0));
        Assert.That(chunksizes, Is.Not.Empty);
        Assert.That(finalOutput, Does.Contain(SuccessMsg));
    }

    [Test]
    public void GetOption()
    {
        Init("http://localhost:5288/GetOption", FilePath);
        IWebElement uploadButton = _driver.FindElement(By.Id("upload-btn"));
        uploadButton.Click();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
            driver => driver.FindElement(By.Id("output")).Text.Contains(SuccessMsg));
        var finalOutput = _driver.FindElement(By.Id("output")).Text;
        var outputs = finalOutput.Split("\n");
        var optLog = outputs.SingleOrDefault(x => x.StartsWith("===CurrentOption:"));
        var opt = JsonSerializer.Deserialize<TusOptions>(optLog.Replace("===CurrentOption:", ""));
        Assert.That(optLog, Is.Not.Null);
        Assert.That(opt, Is.Not.Null);
        Assert.That(finalOutput, Does.Contain(SuccessMsg));
        Assert.That(finalOutput, Does.Contain("===OnProgress"));
        Assert.That(finalOutput, Does.Contain("===OnChunkComplete"));
    }

}