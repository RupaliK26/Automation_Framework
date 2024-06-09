using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Automation_Framework.Configurations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace Automation_Framework.Utilities
{
    public class ReportsHelper : IDisposable
    {
        private ExtentReports _extentReport;
        string reportName = MethodBase.GetCurrentMethod().ReflectedType.Name;
        

        //This will run only once
        public ReportsHelper()
        {
            ////Get the class name of the caller
            //StackFrame frame = new StackFrame(1);
            //string classname1 = frame.GetMethod().DeclaringType.Name;
            //string methodname = frame.GetMethod().Name;
            //var methodInfo = new StackTrace().GetFrame(1).GetMethod();
            //var className = methodInfo.ReflectedType.Name;

            _extentReport = new ExtentReports();
            string folderPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent + "\\ExtentReports\\"; // developer machine
            if (Environment.CurrentDirectory.IndexOf("_FE.Automation.CI") > -1) // pipeline - If the pipeline name changes then we will need to update))
            {
                folderPath = Path.Combine(Environment.CurrentDirectory, "ExtentReports\\");
            }
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            ExtentHtmlReporter htmlReporter = new ExtentHtmlReporter(folderPath);
            _extentReport.AttachReporter(htmlReporter);
            
            _extentReport.AddSystemInfo("OS: ", Environment.OSVersion.ToString());
            _extentReport.AddSystemInfo("Machine Name: ", Environment.MachineName);
            _extentReport.AddSystemInfo("Browser: ", TestSettings.Browser);

            htmlReporter.Config.DocumentTitle = "Extent Report";
            htmlReporter.Config.ReportName = reportName;
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;

        }

        public ExtentReports getExtentReport()
        {
            return _extentReport;
        }

        //This will run only once
        public void Dispose()
        {
            _extentReport.Flush();
        }
    }

    [CollectionDefinition("ReportExtent collection")]
    public class ReportExtentCollection : ICollectionFixture<ReportsHelper>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
