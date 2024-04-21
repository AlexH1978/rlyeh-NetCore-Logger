using System;

namespace Logger
{
    internal static class Helper
    {
        internal static ConsoleColor LogLevelToConsoleColor(LogLevel logLvl)
        {
            switch (logLvl)
                {
                    default:
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                        return ConsoleColor.Gray;

                    case LogLevel.Info:
                        return ConsoleColor.Green;

                    case LogLevel.Warning:
                        return ConsoleColor.Yellow;

                    case LogLevel.Error:
                    case LogLevel.Critical:
                        return ConsoleColor.Red;
                }
        }
    }
}