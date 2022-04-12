using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Baltic.Core.Utils
{
    public static class LoggerSetup
    {
        public static void CreateLogger(bool debugState, string pathFormat)
        {
            const string logTemplate = "{Timestamp:HH:mm:ss} [{Level}{ErrorId}] {EventType:x8}{Message}{NewLine}{Exception}";
            LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch();

            if (debugState)
            {
                levelSwitch.MinimumLevel = LogEventLevel.Debug;
            }

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.ControlledBy(levelSwitch)
               .Enrich.FromLogContext()
               .WriteTo.RollingFile(pathFormat, outputTemplate: logTemplate)
               .WriteTo.LiterateConsole(outputTemplate: logTemplate)
               .CreateLogger();

            Log.Debug("Debug is on");
        }
    }
}