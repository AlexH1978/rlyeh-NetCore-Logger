using System;

namespace Logger
{
    [Flags]
    public enum InitFlags
    {
        None        = 0x00,
        ConsoleLog  = 0x01,
        FileLog     = 0x02,
    }

    public enum LogLevel
    {
        None = 0,
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Critical,
    }

    public enum ServiceState
    {
        Unknown = 0,
        Initializing,
        Initialized,
        Closing,
        Closed,
    }
}