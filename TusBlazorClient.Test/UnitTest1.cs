using System.Net.Mime;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TusBlazorClient.Test;

public class Tests : TestBase
{
    [Test]
    public void Test1()
    {
        _driver.Navigate().GoToUrl("http://localhost:5288/upload");
        WaitUntilLoaded();
        var fileInput = _driver.FindElement(By.Id("file"));
        string filePath = "/home/son/test-image.png";
        fileInput.SendKeys(filePath);
        IWebElement uploadButton = _driver.FindElement(By.Id("upload-btn"));
        uploadButton.Click();
        var finalOutput = _driver.FindElement(By.Id("output")).Text;
        var successMsg = "===Upload Success";
        try
        {
            new WebDriverWait(_driver, TimeSpan.FromSeconds(5)).Until(
                driver => driver.FindElement(By.Id("output")).Text.Contains(successMsg));
        }
        finally
        {
            Assert.That(finalOutput, Does.Contain(successMsg));
        }
    }
}