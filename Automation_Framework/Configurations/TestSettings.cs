using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation_Framework.Configurations
{
    /// <summary>
    /// TestSettings stores appsettings.json configurations as properties
    /// </summary>
    public class TestSettings
    {
        public static string Name { get; set; }
        public static string LoginURL { get; set; }
        public static string ApplicationURL { get; set; }
        public static string HomepageURL { get; set; }
        public static string EdgeLoginURL { get; set; }
        public static string EdgeHomepageURL { get; set; }
        public static string Browser { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string DBConnectionString { get; set; }
        public static bool EnvironmentContainsPowerGrid { get; set; }
        public static string DownloadDirectory { get; set; }



    }
}
