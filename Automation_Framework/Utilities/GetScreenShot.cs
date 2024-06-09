using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Automation_Framework.Utilities
{
  public class GetScreenShot
    {

        public static string Capture(IWebDriver driver, string screenShotName)
        {
            ITakesScreenshot ts = (ITakesScreenshot)driver;
            Screenshot screenshot = ts.GetScreenshot();
            string pth = System.Reflection.Assembly.GetCallingAssembly().Location;
            string finalpth = Path.GetDirectoryName(pth);   // pipeline
            if (pth.LastIndexOf("bin") > -1) //developer machine
            {
                finalpth = pth.Substring(0, pth.LastIndexOf("bin"));
            }

            finalpth = Path.Combine(finalpth, "ErrorScreenshots\\");
            if (!Directory.Exists(finalpth))
            {
                Directory.CreateDirectory(finalpth);
            }
            finalpth += screenShotName + ".png";
            string localpath = new Uri(finalpth).LocalPath;
            screenshot.SaveAsFile(localpath, ScreenshotImageFormat.Png);
            return localpath;
        }

    }
}
