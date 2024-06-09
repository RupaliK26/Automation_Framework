using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Automation_Framework.Utilities
{
    public class LogHelper : IDisposable
    {
        public LogHelper(string reportName)
        {
            string folderPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent +"\\"+ "SerilogLogs\\" + reportName + DateTime.Now.ToString("_MMddyyyy_hhmmtt");
            LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch(LogEventLevel.Debug);
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.ControlledBy(levelSwitch)
                 .WriteTo.File(folderPath + @"\Logs",
                 outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {NewLine}{Exception}",
                 rollingInterval: RollingInterval.Day).CreateLogger();
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}

