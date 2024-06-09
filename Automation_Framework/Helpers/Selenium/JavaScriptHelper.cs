using System.Collections.Generic;
using System;
using OpenQA.Selenium;


//using static com.codeborne.selenide.WebDriverRunner.getWebDriver;

/**
 * @author Brian on 9/19/2016.
 *         <p>
 *         kpace 10/4/2016: updated to return a string. Note also this is mostly a copy/paste from SeleniumPerfTest, so at some
 *         point the duplication should be resolved.
 */

namespace Automation_Framework.Helpers.Selenium
{
    public class JavaScriptHelper {


    private JavaScriptHelper() {
    }


    public static List<LogEntry> getAnySevereJSErrorsFromBrowser(IWebDriver driver) {
        List<LogEntry> jsLogEntries = new List<LogEntry>();


        if (driver != null) {
            try {
                IReadOnlyCollection<LogEntry> logEntries = driver.Manage().Logs.GetLog(LogType.Browser);
                foreach (LogEntry entry in logEntries) {
                    if (entry.Level == LogLevel.Severe) {
                        //ignore those stupid Unrecognized Content-Security-Policy errors that apparently will never get fixed
                        if (!entry.Message.Contains("Unrecognized Content-Security-Policy")) {
                            jsLogEntries.Add(entry);
                        }
                    }
                }
            } catch (WebDriverException e) {
                throw new Exception("Error getting javascript logs. Probably because the driver got closed or was in a bad state.", e);
            }
        }

        return jsLogEntries;
    }

    public static List<string> getAnySevereJSErrorsFromBrowserAsList(IWebDriver driver) {

        List<string> list = new List<string>();
        if (driver != null) {
            try {
                IReadOnlyCollection<LogEntry> logEntries = driver.Manage().Logs.GetLog(LogType.Browser);
                foreach (LogEntry entry in logEntries) {
                    if (entry.Level == LogLevel.Severe)
                    {
                        string s = entry.Level + " " + entry.Message;
                        list.Add(s + "\n");
                    }
                }
            } catch (WebDriverException e) {
                Serilog.Log.Error("Error getting javascript logs. Probably because the driver got closed or was in a bad state: {}", e);
            }
        }

        return list;
    }
}
}
