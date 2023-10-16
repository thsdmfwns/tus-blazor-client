using System.Net.Mime;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TusBlazorClient.Test;

public class E2eTest : TestBase
{
    private const string FilePath = "/home/son/test-image.png";
    private const string ServerUrl = "http://172.17.0.3:8080/files";
    private const string ErrorUrl = "http://172.17.0.3:8080/";
    private const string SuccessMsg = "===Upload Success";

    private void Init(string url, bool isError = false, long? chunkSize = null)
    {
        _driver.Navigate().GoToUrl(url);
        WaitUntilLoaded();
        _driver.FindElement(By.Id("url")).SendKeys(isError ? ErrorUrl : ServerUrl);
        _driver.FindElement(By.Id("file")).SendKeys(FilePath);
        _driver.FindElement(By.Id("chunkSize")).SendKeys(chunkSize.ToString() ?? "null");
    }

    [Test]
    public void Upload()
    {
        Init("http://localhost:5288/upload");
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
        Init("http://localhost:5288/uploadResume", chunkSize: 50000);
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
        Init("http://localhost:5288/ResumeFromPreviousUpload", chunkSize: 50000);
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

}