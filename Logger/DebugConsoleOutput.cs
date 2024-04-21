using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Logger
{
    internal class DebugConsoleOutput
    {
        internal bool DebugLoggingEnabled = false;
        private DebugConsoleOutput()
        {

        }

        internal static DebugConsoleOutput GetObject()
        {
            return new DebugConsoleOutput();
        }

        private class LogDataDto
        {
            internal LogLevel LogLvl { get; private set; }
            internal string Message { get; private set; }
            internal string Path { get; private set; }
            internal string Caller { get; private set; }
            internal int LineNo { get; private set; }
            internal int ThreadId { get; private set; }

            internal LogDataDto(LogLevel logLvl, string message, string path, string caller, int lineNo, int threadId)
            {
                LogLvl = logLvl;
                Message = message;
                Path = path;
                Caller = caller;
                LineNo = lineNo;
                ThreadId = threadId;
            }
        }

#pragma warning disable CS8625
        internal void WriteDebugOutput(LogLevel logLvl, string logStr, [CallerFilePath] string path = null, [CallerMemberName] string caller = null, [CallerLineNumber] int lineNo = 0)
        {
#if DEBUG
            if(!DebugLoggingEnabled || String.IsNullOrWhiteSpace(logStr))
                return;

            WriteToConsole(new LogDataDto(logLvl, logStr, path, caller, lineNo, Thread.CurrentThread.ManagedThreadId));
#endif
        }

        internal void WriteDebugException(Exception e, [CallerFilePath] string path = null, [CallerMemberName] string caller = null, [CallerLineNumber] int lineNo = 0)
        {
#if DEBUG
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if(!DebugLoggingEnabled || e == null)
                return;

            WriteToConsole(new LogDataDto(LogLevel.Error, e.ToString(), path, caller, lineNo, Thread.CurrentThread.ManagedThreadId));
#endif
        }

        internal void WriteDebugOutput(LogLevel logLvl, Func<string> logFunc, [CallerFilePath] string path = null, [CallerMemberName] string caller = null, [CallerLineNumber] int lineNo = 0)
        {
#if DEBUG
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if(!DebugLoggingEnabled || logFunc == null)
                return;

            WriteToConsole(new LogDataDto(logLvl, logFunc.Invoke(), path, caller, lineNo, Thread.CurrentThread.ManagedThreadId));
#endif
        }
#pragma warning restore CS8625
        private void WriteToConsole(LogDataDto logData)
        {
#if DEBUG
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if(!DebugLoggingEnabled|| logData == null || String.IsNullOrWhiteSpace(logData.Message))
                return;

            try
            {
                var str = $"DEBUG-{DateTime.Now:MM/dd/yyyy-HH:mm:ss.fff} [{logData.LogLvl}] {Path.GetFileName(logData.Path)}.{logData.Caller}(L:{logData.LineNo}/T:{logData.ThreadId}) - {logData.Message}";

                var currentColor = Console.ForegroundColor;
                
                switch (logData.LogLvl)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;

                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case LogLevel.Error:
                    case LogLevel.Critical:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }

                Console.WriteLine(str);
                Console.ForegroundColor = currentColor;
            }
            catch
            {
                // ignored
            }
#endif
        }
    }
}