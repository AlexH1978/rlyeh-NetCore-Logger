using System;
using System.IO;

namespace Logger
{
    internal class LoggerObj
    {
        public LogLevel Level{ get; private set; }
        private DateTime TimeStamp { get; set; }
        public string? Context { get; private set; }
        private object? State { get; set; }
        private Exception? Exception { get; set; }
        private Func<object>? Function { get; set; }
        private int ThreadId { get; set; }
        private string FileName { get; set; }
        private string CallerName { get; set; }
        private int LineNumber { get; set; }

        public LoggerObj(LogLevel level, DateTime timeStamp, string? context, object? state, Exception? exception, Func<object>? function, int threadId, string fileName, string callerName, int lineNumber)
        {
            Level = level;
            TimeStamp = timeStamp;
            Context = context;
            State = state;
            Exception = exception;
            Function = function;
            ThreadId = threadId;
            FileName = Path.GetFileName(fileName);
            CallerName = callerName;
            LineNumber = lineNumber;
        }

        public override string ToString()
        {
            string msg = String.Empty;
            if(State != null)
                msg = State?.ToString() ?? String.Empty;

            else if(Exception != null)
                msg = $"{Exception.Message}{Environment.NewLine}{Exception.StackTrace}";

            else if(Function != null)
                msg = Function?.Invoke().ToString() ?? String.Empty;

            return $"{TimeStamp:MM/dd/yyyy-HH:mm:ss.fff} [{Level}] {FileName}.{CallerName}(L:{LineNumber} T:{ThreadId}): {msg}{Environment.NewLine}";
        }
    }
}