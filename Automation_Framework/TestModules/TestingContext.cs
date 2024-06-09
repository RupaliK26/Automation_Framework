using AventStack.ExtentReports;
using Automation_Framework.PageObjects;
using Automation_Framework.Utilities;
using OpenQA.Selenium;


namespace Automation_Framework.TestModules
{
    class TestingContext : ITestingContext
    {
        public TestingContext(IWebDriver driver, BrowserSetup browser, ExtentReports extentReport)
        {
            Driver = driver;
            BrowserSetup = browser;
            ExtentReport = extentReport;
        }

        public BrowserSetup BrowserSetup { get; }
        public IWebDriver Driver { get; }
        public ExtentReports ExtentReport { get; }
        public ExtentTest ExtentTest { get; set; }
        public string ReportName { get; set; }
    }
}
namespace Automation_Framework.PageObjects
{ 
    public interface ITestingContext
    {
        BrowserSetup BrowserSetup { get; }
        IWebDriver Driver { get; }
        ExtentReports ExtentReport { get; }
        ExtentTest ExtentTest { get; set; }
        string ReportName { get; set; }
    }
}
