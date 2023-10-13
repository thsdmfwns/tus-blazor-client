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
        // 웹 드라이버 초기화
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
}