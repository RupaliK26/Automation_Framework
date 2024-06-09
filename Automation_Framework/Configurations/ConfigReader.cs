using Automation_Framework.Configurations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Automation_Framework.Configurations
.Configurations
{
    /// <summary>
    /// Reads configurations from appsettings.json and assigns them to TestSettings setters
    /// </summary>
    public class ConfigReader
    {
        public ConfigReader()
        {
            SetFrameworkSettings();
        }
        public static void SetFrameworkSettings()
        {
            var testEnv = Environment.GetEnvironmentVariable("QAEnvironment");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{testEnv}.json", optional: true)
                .Build();

            TestSettings.Name = config.GetSection("TestSettings").GetValue<string>("Name");
            TestSettings.LoginURL = config.GetSection("TestSettings").GetValue<string>("LoginURL");
            TestSettings.ApplicationURL = config.GetSection("TestSettings").GetValue<string>("ApplicationURL");
            TestSettings.HomepageURL = config.GetSection("TestSettings").GetValue<string>("HomepageURL");
            TestSettings.EdgeLoginURL = config.GetSection("TestSettings").GetValue<string>("EdgeLoginURL");
            TestSettings.EdgeHomepageURL = config.GetSection("TestSettings").GetValue<string>("EdgeHomepageURL");
            TestSettings.Browser = config.GetSection("TestSettings").GetValue<string>("Browser");
            TestSettings.Username = config.GetSection("TestSettings").GetValue<string>("Username");
            TestSettings.Password = config.GetSection("TestSettings").GetValue<string>("Password");
            TestSettings.DBConnectionString = config.GetSection("TestSettings").GetValue<string>("DBConnectionString");
            TestSettings.EnvironmentContainsPowerGrid = config.GetSection("TestSettings").GetValue<bool>("EnvironmentContainsPowerGrid");

        }
    }
}
