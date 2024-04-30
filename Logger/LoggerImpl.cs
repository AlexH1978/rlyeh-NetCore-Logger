using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Logger
{
    internal partial class LoggerService : ILogger
    {
        private void Log(LoggerObj loggerObj)
        {
            if(loggerObj.Level < _logLevel)
                return;

            EnqueueLoggerObj(loggerObj);
        }

        #region ILogger
        public void Init(LogLevel logLevel, InitFlags flags, string fullQualifiedFileName, int rolloverSize = 0)
        {
            if(logLevel != LogLevel.None && flags == InitFlags.None)
                throw new ArgumentException($"Invalid args {nameof(logLevel)}, {nameof(flags)}");

            if((flags & InitFlags.FileLog) != 0 && String.IsNullOrWhiteSpace(fullQualifiedFileName))
                throw new ArgumentException($"Invalid args {nameof(flags)}, {nameof(fullQualifiedFileName)}");

            try
            {
                _debug.WriteDebugOutput(LogLevel.Debug, () => $"Start logLevel:{logLevel} flags:{flags} fullQualifiedFileName:{fullQualifiedFileName}");
                
                lock(_syncRoot)
                {
                    if(_state == ServiceState.Initializing ||
                       _state == ServiceState.Initialized || 
                       _state == ServiceState.Closing)
                        throw new ApplicationException($"Invalid state:{_state}!");

                    _state = ServiceState.Initializing;
                }

                StopLogThread();
                _flags = flags;
                _logLevel = logLevel;
                _rolloverSize = rolloverSize;
                
                if((_flags & InitFlags.FileLog) != 0)
                {
                    try
                    {
                        string? dir = Path.GetDirectoryName(fullQualifiedFileName);
                        if(dir == null)
                            throw new ApplicationException("Invalid directory");
                        if(!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        _logFile = fullQualifiedFileName;
                        _logFileOriginal = fullQualifiedFileName;
                    }
                    catch(Exception e)
                    {
                        _debug.WriteDebugException(e);
                        throw;
                    }
                }
                else
                    _logFile = null;

                StartLogThread();

                lock(_syncRoot)
                {
                    _state = ServiceState.Initialized;
                }
            }
            catch (Exception e)
            {
                _debug.WriteDebugException(e);

                lock(_syncRoot)
                {
                    _state = ServiceState.Closed;
                }

                throw;
            }
            finally
            {
                _debug.WriteDebugOutput(LogLevel.Debug, "End");
            }
        }

        public void SetLogLevel(LogLevel logLevel)
        {
            try
            {
                _debug.WriteDebugOutput(LogLevel.Debug, "Start");
                VerifyInitialisation();
                _logLevel = logLevel;
            }
            catch (Exception e)
            {
                _debug.WriteDebugException(e);
                throw;
            }
            finally
            {
                _debug.WriteDebugOutput(LogLevel.Debug, "End");
            }
        }

        public void Close(bool force = false)
        {
            try
            {
                _debug.WriteDebugOutput(LogLevel.Debug, "Start");

                lock(_syncRoot)
                {
                    if(_state == ServiceState.Closing || 
                    _state == ServiceState.Closed)
                        throw new ApplicationException($"Invalid state:{_state}!");

                    _state = ServiceState.Closing;
                }

                StopLogThread(force);
            }
            catch (Exception e)
            {
                _debug.WriteDebugException(e);
            }
            finally
            {
                lock(_syncRoot)
                {
                    _state = ServiceState.Closed;
                }
                _debug.WriteDebugOutput(LogLevel.Debug, "End");
            }
        }
        
        public void LogCritical(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Critical, DateTime.Now, null, message, null, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogCritical(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Critical, DateTime.Now, null, null, null, function, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogCritical(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Critical, DateTime.Now, null, null, exception, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogDebug(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Debug, DateTime.Now, null, message, null, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogDebug(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Debug, DateTime.Now, null, null, null, function, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogDebug(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Debug, DateTime.Now, null, null, exception, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogError(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Error, DateTime.Now, null, message, null, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogError(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Error, DateTime.Now, null, null, null, function, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogError(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Error, DateTime.Now, null, null, exception, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogInfo(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Info, DateTime.Now, null, message, null, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogInfo(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Info, DateTime.Now, null, null, null, function, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogInfo(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Info, DateTime.Now, null, null, exception, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }
       
        public void LogTrace(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Trace, DateTime.Now, null, message, null, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogTrace(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Trace, DateTime.Now, null, null, null, function, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogTrace(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Trace, DateTime.Now, null, null, exception, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogWarning(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Warning, DateTime.Now, null, message, null, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogWarning(Func<object> function, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Warning, DateTime.Now, null, null, null, function, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }

        public void LogWarning(Exception exception, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(new LoggerObj(LogLevel.Warning, DateTime.Now, null, null, exception, null, Thread.CurrentThread.ManagedThreadId, file, member, line));
        }
        #endregion
    }
}