using Automation_Framework.Configurations;
using Automation_Framework.Configurations.Configurations;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace Automation_Framework.Utilities
{
    public class BrowserSetup : IDisposable
    {
        private IWebDriver _driver;

        public BrowserSetup()
        {
            if (string.IsNullOrWhiteSpace(TestSettings.Browser))
            {
                ConfigReader.SetFrameworkSettings();
            }
            string browserType = TestSettings.Browser;
            if (string.IsNullOrWhiteSpace(TestSettings.DownloadDirectory))
            {
                TestSettings.DownloadDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            }

            if (browserType == "Chrome")
            {
                new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
                ChromeOptions chromeOptions = new ChromeOptions();                
                chromeOptions.AddExcludedArgument("enable-automation");
                chromeOptions.AddArgument("--incognito");
                chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
                chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
                chromeOptions.AddUserProfilePreference("download.default_directory", TestSettings.DownloadDirectory);
                _driver = new ChromeDriver(chromeOptions);
                _driver.Manage().Window.Maximize();
            }
            else if (browserType == "Headless Chrome")
            {
                new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
                ChromeOptions chromeOptions = new ChromeOptions();
                /*
                  Added next two User Profile Preferences to make sure that the pdf documents are automatically downloaded rather than opening directly in the browser.
                  File would be automatically downloaded to the default directy.
                  This behaviour is only for Headless mode.      
               */
                chromeOptions.AddUserProfilePreference("download.default_directory", TestSettings.DownloadDirectory);
                chromeOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
                chromeOptions.AddArguments("--headless=new");
                chromeOptions.AddArguments("--window-size=1920,1080");
                chromeOptions.AddArguments("--start-maximized");
                chromeOptions.AddExcludedArgument("enable-automation");
                chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
                chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
                chromeOptions.AddUserProfilePreference("download.default_directory", TestSettings.DownloadDirectory);

                _driver = new ChromeDriver(chromeOptions);
                _driver.Manage().Window.Maximize();
            }
            else if (browserType == "IE")
            {

            }
        }

        public IWebDriver getDriver()
        {
            return _driver;
        }

        public void Dispose()
        {
            // Clean up any files created.
            if (!string.IsNullOrWhiteSpace(TestSettings.DownloadDirectory))
            {
                DirectoryInfo dir = new DirectoryInfo(TestSettings.DownloadDirectory);
                if (dir.Exists)
                {
                    dir.Delete(true); // delete everything in it.
                }
            }
            _driver.Quit();
        }

    }
}
