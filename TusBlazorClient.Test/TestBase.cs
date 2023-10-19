using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace TusBlazorClient.Test;

public class TestBase
{
    protected IWebDriver _driver;
    
    [SetUp]
    public void Setup()
    {
        _driver = new FirefoxDriver();
    }
    
    [TearDown]
    public void Teardown()
    {
        _driver.Quit();
        _driver.Dispose();
        
    }
    
    
    protected void WaitUntilLoaded()
    {
        _driver.Manage().Window.Maximize();
        new WebDriverWait(_driver, TimeSpan.FromSeconds(30)).Until(
            driver => driver.FindElement(By.Id("DemoName")));
        Task.Delay(1000).Wait();
    }

    private const string ServerUrl = "http://172.17.0.3:8080/files";
    private const string ErrorUrl = "http://172.17.0.3:8080/";
    protected const string SuccessMsg = "===Upload Success";

    protected void Init(string url, string filePath, bool isError = false, long? chunkSize = null)
    {
        _driver.Navigate().GoToUrl(url);
        WaitUntilLoaded();
        _driver.FindElement(By.Id("url")).SendKeys(isError ? ErrorUrl : ServerUrl);
        _driver.FindElement(By.Id("file")).SendKeys(filePath);
        var chunkSizeStr = chunkSize.ToString();
        if (string.IsNullOrEmpty(chunkSizeStr))
        {
            chunkSizeStr = "null";
        }
        _driver.FindElement(By.Id("chunkSize")).SendKeys(chunkSizeStr);
    }
}