using System;
using System.Runtime.CompilerServices;

namespace Logger
{
    public interface ILogger
    {
        /// <summary>
        /// Initialize, stop or restart the Logger.
        /// If the logger is running, the logger will restarted with the new parameters.
        /// If the parameter <paramref name="logLvl"/> is 'None', tel logger will be stopped/> 
        /// </summary>
        /// <param name="logLvl"></param>
        /// <param name="flags"></param>
        /// <param name="fullQualifiedFileName"></param>
        void Init(LogLevel logLvl, InitFlags flags, string fullQualifiedFileName);

        /// <summary>
        /// Initialize, stop or restart the Logger.
        /// If the logger is running, the logger will restarted with the new parameters.
        /// If the parameter <paramref name="logLvl"/> is 'None', tel logger will be stopped/> 
        /// </summary>
        /// <param name="logLvl"></param>
        /// <param name="flags"></param>
        /// <param name="fullQualifiedFileName"></param>
        /// <param name="rolloverSize">in mega byte</param>
        void Init(LogLevel logLvl, InitFlags flags, string fullQualifiedFileName, int rolloverSize);

        /// <summary>
        /// Change the current logging level on the fly, or stopping the logging if the <paramref name="logLvl"/> is 'None'.
        /// </summary>
        /// <param name="logLvl"></param>
        void SetLogLevel(LogLevel logLvl);

        /// <summary>
        /// Is stopping the logging and free all resources.
        /// </summary>
        void Close(bool force = false);
        
        void LogTrace(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogTrace(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogTrace(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogDebug(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogDebug(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogDebug(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogInfo(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogInfo(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogInfo(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogWarning(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogWarning(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogWarning(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogError(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogError(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogError(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogCritical(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogCritical(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        void LogCritical(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
    }
}