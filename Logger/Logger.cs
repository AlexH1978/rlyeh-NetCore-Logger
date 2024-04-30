using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{
    internal partial class LoggerService
    {
        private ServiceState _state;
        private readonly object _syncRoot = new();
        private readonly ConcurrentQueue<LoggerObj> _logQueue = new();
        private readonly DebugConsoleOutput _debug;
        private bool _doLogging;
        private Task? _logTask;
        private FileStream? _fileStream;
        private LogLevel _logLevel = LogLevel.None; 
        private string? _logFileOriginal;
        private string? _logFile;
        private InitFlags _flags = InitFlags.None;
        private DateTime _fileLastFlush = DateTime.MinValue;
        private uint _fileRolloverSuffix ;
        private bool _timerInvoke;
        private int _rolloverSize;
        private Timer? _rolloverTimer;

        internal bool EnableDebugLogging 
        { 
            set => _debug.DebugLoggingEnabled = value;
            get => _debug.DebugLoggingEnabled; 
        }

        private bool FileLogEnabled => (_flags & InitFlags.FileLog) != 0 && !String.IsNullOrWhiteSpace(_logFile);
        
        internal LoggerService()
        {
            _debug = DebugConsoleOutput.GetObject();
        }

        private void VerifyInitialisation()
        {
            lock(_syncRoot)
            {
                if(_state != ServiceState.Initialized)
                    throw new ApplicationException("Not initialized!");
            }
        }

        private void RolloverTimerElapsed(object? obj)
        {
            if(_timerInvoke)
                return;

            lock(_syncRoot)
            {
                if(_timerInvoke)
                    return;
                _timerInvoke = true;
            }

            try
            {
                long? size = _fileStream?.Length;
                if(size != null && (size / 2000) > 1)
                {
                    _debug.WriteDebugOutput(LogLevel.Info, "========= ROLLOVER");
                    StopLogThread();
                    _logFile = $"{_logFileOriginal}-{_fileRolloverSuffix++}";
                    StartLogThread();
                }
            }
            catch(Exception e)
            {
                _debug.WriteDebugException(e);
                StopLogThread();
            }
            finally
            {
                lock(_syncRoot)
                {
                    _timerInvoke = false;
                }
            }
        }

        private void StartLogThread()
        {
            lock(_syncRoot)
            {
                if(_doLogging)
                    return;

                _doLogging = true;
            }
            
            if(FileLogEnabled && _rolloverSize > 0)
                _rolloverTimer = new Timer(RolloverTimerElapsed, null, TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(10000));

            _logTask = Task.Run(LogWorker);
        }

        private void LogWorker()
        {
            try
            {
                _debug.WriteDebugOutput(LogLevel.Info, $"Start stream {DateTime.Now:MM/dd/yyyy-HH:mm:ss.fff}");
                do
                {
                    
                    if(_logQueue.TryDequeue(out var log))
                    {
                        WriteLog(log);
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                while(_doLogging || _logQueue.Count > 0);
                
                SaveCloseFileStream();
                _debug.WriteDebugOutput(LogLevel.Info, $"Stop stream {DateTime.Now:MM/dd/yyyy-HH:mm:ss.fff}");
            }
            catch(Exception e)
            {
                _debug.WriteDebugException(e);
            }
        }

        private void SaveCloseFileStream()
        {
            try
            {
                _fileStream?.Flush(true);
                _fileStream?.Close();
            }
            catch(Exception e)
            {
                _debug.WriteDebugException(e);
            }
            finally
            {
                _fileStream = null;
                _fileLastFlush = DateTime.MinValue;
            }
        }

        private void StopLogThread(bool force = false)
        {
            lock(_syncRoot)
            {
                if(!_doLogging)
                    return;
                
                _doLogging = false;
            }

            try
            {
                _rolloverTimer?.Dispose();

                if(force)
                    _logQueue.Clear();
                _logTask?.Wait();
            }
            catch(Exception e)
            {
                _debug.WriteDebugException(e);
            }
            finally
            {
                SaveCloseFileStream();
            }
        }

        private void EnqueueLoggerObj(LoggerObj loggerObj)
        {
            if(_state == ServiceState.Initialized)
                _logQueue.Enqueue(loggerObj);
        }

        private void WriteLog(LoggerObj loggingInfo)
        {
            try
            {
                string logEntry = loggingInfo.ToString();
                if (String.IsNullOrWhiteSpace(logEntry))
                    return;
                
                if((_flags & InitFlags.FileLog) != 0 && !String.IsNullOrWhiteSpace(_logFile))
                {
                    if(_fileStream == null)
                    {
                        try
                        {
                            _fileLastFlush = DateTime.Now;
                            _fileStream = File.OpenWrite(_logFile);
                        }
                        catch(Exception e)
                        {
                            _debug.WriteDebugException(e);
                            SaveCloseFileStream();
                        }
                    }
                    try
                    {
                        var bytes = new UTF8Encoding().GetBytes(logEntry);
                        _fileStream?.Write(bytes, 0, bytes.Length);
                        
                        if((_fileLastFlush + TimeSpan.FromMilliseconds(500)) < DateTime.Now)
                        {
                            _fileStream?.Flush(true);
                            _fileLastFlush = DateTime.Now;
                            _debug.WriteDebugOutput(LogLevel.Info, "========= FLUSH");
                        }

                    }
                    catch(Exception e)
                    {
                        _debug.WriteDebugException(e);
                        SaveCloseFileStream();
                    }
                }

                if ((_flags & InitFlags.ConsoleLog) == 0) 
                    return;
                
                var current = Console.ForegroundColor;
                Console.ForegroundColor = Helper.LogLevelToConsoleColor(loggingInfo.Level);
                Console.Write(logEntry);
                Console.ForegroundColor = current;
            }
            catch (Exception e)
            {
                _debug.WriteDebugException(e);
            }
        }
    }
}

