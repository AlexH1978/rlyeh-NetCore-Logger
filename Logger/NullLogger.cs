using System;

namespace Logger
{
    public class NullLogger : ILogger
    {
        public void Init(LogLevel logLvl, InitFlags flags, string fullQualifiedFileName)
        {
        }

        public void Init(LogLevel logLvl, InitFlags flags, string fullQualifiedFileName, int rolloverSize)
        {
        }

        public void SetLogLevel(LogLevel logLvl)
        {
        }

        public void Close(bool force = false)
        {
        }

        public void LogTrace(string message, string file = "", string member = "", int line = 0)
        {
        }

        public void LogTrace(Func<object> function, string file = "", string member = "", int line = 0)
        {
        }

        public void LogTrace(Exception exception, string file = "", string member = "", int line = 0)
        {
        }

        public void LogDebug(string message, string file = "", string member = "", int line = 0)
        {
        }

        public void LogDebug(Func<object> function, string file = "", string member = "", int line = 0)
        {
        }

        public void LogDebug(Exception exception, string file = "", string member = "", int line = 0)
        {
        }

        public void LogInfo(string message, string file = "", string member = "", int line = 0)
        {
        }

        public void LogInfo(Func<object> function, string file = "", string member = "", int line = 0)
        {
        }

        public void LogInfo(Exception exception, string file = "", string member = "", int line = 0)
        {
        }

        public void LogWarning(string message, string file = "", string member = "", int line = 0)
        {
        }

        public void LogWarning(Func<object> function, string file = "", string member = "", int line = 0)
        {
        }

        public void LogWarning(Exception exception, string file = "", string member = "", int line = 0)
        {
        }

        public void LogError(string message, string file = "", string member = "", int line = 0)
        {
        }

        public void LogError(Func<object> function, string file = "", string member = "", int line = 0)
        {
        }

        public void LogError(Exception exception, string file = "", string member = "", int line = 0)
        {
        }

        public void LogCritical(string message, string file = "", string member = "", int line = 0)
        {
        }

        public void LogCritical(Func<object> function, string file = "", string member = "", int line = 0)
        {
        }

        public void LogCritical(Exception exception, string file = "", string member = "", int line = 0)
        {
        }
    }
}