using NLog;
using NLog.LayoutRenderers;
using System.Text;

namespace AchieveTrack.NLogRenderer;

// NOTE: https://tewarid.github.io/2018/06/01/logging-to-syslog-using-nlog.html

public enum SyslogFacility
{
    UserLevelMessages = 1
};

public enum SyslogSeverity
{
    Emergency = 0,      // system is unusable
    Alert = 1,          // action must be taken immediately
    Critical = 2,       // critical conditions
    Error = 3,          // error conditions
    Warning = 4,        // warning conditions
    Notice = 5,         // normal but significant condition
    Informational = 6,  // informational messages
    Debug = 7           // debug-level messages
};

[LayoutRenderer("syslogpriority")]
public class SyslogPriorityRenderer : LayoutRenderer
{
    readonly Dictionary<LogLevel, SyslogSeverity> NLogLevelToSyslogSeverity =
        new()
        {
            { LogLevel.Debug, SyslogSeverity.Debug },
            { LogLevel.Error, SyslogSeverity.Error },
            { LogLevel.Fatal, SyslogSeverity.Critical },
            { LogLevel.Info, SyslogSeverity.Informational },
            { LogLevel.Trace, SyslogSeverity.Debug },
            { LogLevel.Warn, SyslogSeverity.Warning }
        };

    protected override void Append(StringBuilder builder, LogEventInfo logEvent)
    {
        builder.Append($"<{(int)SyslogFacility.UserLevelMessages * 8 + NLogLevelToSyslogSeverity[logEvent.Level]}>");
    }
}