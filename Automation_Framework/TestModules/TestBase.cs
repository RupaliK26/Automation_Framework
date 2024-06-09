using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Automation_Framework.Utilities;
using AventStack.ExtentReports;
using OpenQA.Selenium;
using Automation_Framework.Configurations;
using Automation_Framework.PageObjects;
using Xunit.Abstractions;
using Automation_Framework.Configurations;
using Automation_Framework.PageObjects;
using Automation_Framework.TestModules;
using Automation_Framework.Utilities;

namespace Automation_Framework.TestModules
{
    public class TestBase : IDisposable
    {
        public BrowserSetup browserSetup;
        public IWebDriver driver;
        public ExtentTest extentTest = null;
        public ExtentReports extentReport = null;
        public string reportName = MethodBase.GetCurrentMethod().ReflectedType.Name;
        public ITest testOutput = null;
        private ITestingContext testingContext;
        public ITestingContext TestingContext { get { return testingContext; } }
        public string ScreenshotName { get; set; }

        public TestBase(ReportsHelper reportsHelper, string reportTestName, BrowserSetup browserSetupParm = null)
        {
            InitializeBase(reportsHelper, reportTestName, browserSetupParm);

        }

        private void InitializeBase(ReportsHelper reportsHelper, string reportTestName, BrowserSetup browserSetupParm)
        {
            if (browserSetupParm == null)
            {
                browserSetup = new BrowserSetup();
            }

            reportName = reportTestName;
            extentReport = reportsHelper.getExtentReport();
            driver = browserSetup.getDriver();
            ScreenshotName = reportTestName;

            LogHelper log = new LogHelper(reportName);
            driver.Navigate().GoToUrl(TestSettings.LoginURL);
            Serilog.Log.Information("The Url is : " + TestSettings.LoginURL);
            Login.PerformLogin(driver, TestSettings.Username, TestSettings.Password);

            // object to pass as much of the above as necessary to the generic job screen class for it to be able to work
            testingContext = new TestingContext(driver, browserSetup, extentReport);
        }

        public TestBase(ReportsHelper reportsHelper, ITestOutputHelper output, string reportTestName, BrowserSetup browserSetupParm = null)
        {
            InitializeBase(reportsHelper, reportTestName, browserSetupParm);

            var type = output.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            testOutput = (ITest)testMember.GetValue(output);
        }

        public void ReportExceptionAndFail(ExtentTest extentTest, Exception e, string testMethodFailMessage)
        {
            // Capture screen shot of error before logging failure
            string errorImageFile = GetScreenShot.Capture(driver, ScreenshotName + " error");
            //Get the relative path of the screenshot to the report.
            errorImageFile = "..\\ErrorScreenshots\\" + System.IO.Path.GetFileName(errorImageFile);
            if (extentTest.Status == Status.Fail)
            {
                if (e.GetType() == typeof(FEGroupAssertExceptions))
                    extentTest.Log(Status.Fail, testMethodFailMessage);
                else
                    extentTest.Fail(testMethodFailMessage + " <br>" + e.StackTrace.ToString()).AddScreenCaptureFromPath(errorImageFile);
            }
            else
            {
                Serilog.Log.Error("Error Message: " + e.Message.ToString());
                extentTest.Log(Status.Fail, testMethodFailMessage + " <br>" + e.Message.ToString());
                Serilog.Log.Error(testMethodFailMessage);
                extentTest.Fail("Test Fail <br> " + e.StackTrace).AddScreenCaptureFromPath(errorImageFile);
                Serilog.Log.Error("Test Fail " + e.StackTrace);
            }
        }

        public void Dispose()
        {
            browserSetup.Dispose();
        }

    }
}
