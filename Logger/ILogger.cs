using System;
using System.Runtime.CompilerServices;
// ReSharper disable InvalidXmlDocComment

namespace Logger
{
    public interface ILogger
    {
        /// <summary>
        /// Starts, stops or restarts the logging mechanism.
        /// If logging has already been initialized, logging is restarted.
        /// If the parameter <paramref name="logLevel"/> has the value 'None'. 
        /// </summary>
        /// <param name="logLevel">Logging level from 'None' to 'Critical'</param>
        /// <param name="flags">Operating mode 'FileLog' (Write to file) and/or 'ConsoleLog' (Console output).
        /// The modes can be combined.</param>
        /// <param name="fullQualifiedFileName">Path including filename, if the 'FileLog' mode is to be activated</param>
        /// <param name="rolloverSize">Defines the maximum file size. If this size is reached, a new file is written to.</param>
        void Init(LogLevel logLevel, InitFlags flags, string fullQualifiedFileName, int rolloverSize = 0);

        /// <summary>
        /// Changes the current logging level during operation. Deactivates logging if the <paramref name="logLevel"/> is set for 'None'.
        /// </summary>
        /// <param name="logLevel">Logging level from 'None' to 'Critical'</param>
        void SetLogLevel(LogLevel logLevel);

        /// <summary>
        /// Stops logging and releases all resources.
        /// </summary>
        /// <param name="force">If 'true', all logging operations are canceled without writing the cache.</param>
        void Close(bool force = false);
        
        /// <summary>
        /// Writes the message string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="message"></param>
        void LogTrace(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);

        /// <summary>
        /// Execute the function and write the generated string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="function"></param>
        void LogTrace(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Write the exception information to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="exception"></param>
        void LogTrace(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Writes the message string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="message"></param>
        void LogDebug(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Execute the function and write the generated string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="function"></param>
        void LogDebug(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        // <summary>
        /// Write the exception information to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="exception"></param>
        void LogDebug(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Writes the message string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="message"></param>
        void LogInfo(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Execute the function and write the generated string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="function"></param>
        void LogInfo(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        // <summary>
        /// Write the exception information to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="exception"></param>
        void LogInfo(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Writes the message string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="message"></param>
        void LogWarning(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Execute the function and write the generated string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="function"></param>
        void LogWarning(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        // <summary>
        /// Write the exception information to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="exception"></param>
        void LogWarning(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Writes the message string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="message"></param>
        void LogError(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Execute the function and write the generated string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="function"></param>
        void LogError(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        // <summary>
        /// Write the exception information to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="exception"></param>
        void LogError(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Writes the message string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="message"></param>
        void LogCritical(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        /// <summary>
        /// Execute the function and write the generated string to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="function"></param>
        void LogCritical(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        
        // <summary>
        /// Write the exception information to the logging destination if the corresponding log level has been set.
        /// </summary>
        /// <param name="exception"></param>
        void LogCritical(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
    }
}