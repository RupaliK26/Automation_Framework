using System;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Automation_Framework.Utilities;
using Automation_Framework.PageObjects;
using Automation_Framework.Configurations;
using Microsoft.VisualStudio.TestPlatform.Utilities;


/**
 * @author Rupali Kashyap
 */

namespace Automation_Framework.TestModules.Application1.Module1.SubModule1
{
    [Collection("ReportExtent collection")]
    public class SampleTest : TestBase
    {
        

        // Define Page objectValue instance variables
           public Login loginPage;

        // @BeforeClass

        public SampleTest(ReportsHelper reportsHelper ,ITestOutputHelper output) : base(reportsHelper, output, MethodBase.GetCurrentMethod().ReflectedType.Name)
        {
            // Initializing page objectValue class
                               
            
        }

      
        //@Jira(ticketNumber = "TCM-1")
        [Fact]
        //    @Test
        public void gmailLogin()
        {
            extentTest = extentReport.CreateTest(MethodBase.GetCurrentMethod().Name).Info("This test is to verifyGmailLogin");
            ScreenshotName = testOutput.TestCase.DisplayName;
            try
            {
                //loginPage.PerformLogin(driver, TestSettings.Username, TestSettings.Username,TestSettings.Password);
               // Already doing in base class
            }
            catch (Exception e)
            {
                ReportExceptionAndFail(extentTest, e, "gmail login failed");
                throw;
            }
           extentTest.Pass("Successfully passed gmailLogin");

        }
    }
}
