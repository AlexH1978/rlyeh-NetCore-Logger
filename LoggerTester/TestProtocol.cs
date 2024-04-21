using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Logger;

namespace LoggerTester
{
    internal class TestProtocol
    {
        private ILogger? _logger;
        private readonly Stopwatch _completeTime = new Stopwatch();
        private readonly Stopwatch _sectionTime = new Stopwatch();
        
        public void StartTest()
        {
            // Add remove old logs
            WriteStartingHeader();
            
            InitializingLoggerService($"/home/alex/Development/TestOutput/{DateTime.Now:MM-dd-yyyy_HH-mm-ss}-testLog_1.log", InitFlags.FileLog);

            BasicLogs();
            
            CloseLoggerService(false);

            InitializingLoggerService($"/home/alex/Development/TestOutput/{DateTime.Now:MM-dd-yyyy_HH-mm-ss}-testLog_2.log", InitFlags.FileLog);

            BasicLogs_90000();

            CloseLoggerService(false);

            InitializingLoggerService($"/home/alex/Development/TestOutput/{DateTime.Now:MM-dd-yyyy_HH-mm-ss}-testLog_3.log", InitFlags.FileLog);

            BasicLogs_5_Threads_90000().Wait();

            CloseLoggerService(false);
            
            InitializingLoggerService($"/home/alex/Development/TestOutput/{DateTime.Now:MM-dd-yyyy_HH-mm-ss}-testLog_4.log", InitFlags.FileLog, 3);

            BasicLogs_90000_Delayed();

            CloseLoggerService(false);

            WriteFinishHeader();
        }
        
        private void InitializingLoggerService(string fileName, InitFlags initFlags, int size = 0)
        {
            WriteInfo($"Initializing the logger service fileName:{fileName} initFlags:{initFlags}");
            try
            {
                _sectionTime.Start();
                _logger = LoggerFactory.GetLogger(true);
                
                _logger.Init(LogLevel.Trace,
                                    InitFlags.FileLog,
                                    fileName,
                                    size);
            }
            catch(Exception e)
            {
                WriteError(e);
            }

            if(_logger == null)
            {
                WriteError($"{nameof(_logger)} is null!");
                return;
            }

            _sectionTime.Stop();
            WriteInfo($"Logger service is Initialized {_sectionTime.ElapsedMilliseconds}ms");
        }

        private void CloseLoggerService(bool force)
        {
            try
            {
                _sectionTime.Start();
                WriteInfo("Close the logger service");
                _logger?.Close(force);
            }
            catch (Exception e)
            {
                WriteError(e);
            }
            _sectionTime.Stop();
            WriteInfo($"Logger service closed {_sectionTime.ElapsedMilliseconds}ms!");
        }
        private void BasicLogs(string param = "")
        {
            try
            {
                if(!String.IsNullOrWhiteSpace(param))
                    _logger?.LogTrace(() => $"========= {param} =========");
                else
                {
                    WriteInfo("-> Write basic logs");
                    _sectionTime.Start();
                    _logger?.LogTrace("Trace 1");
                }

                int ph = 3;
                try { throw new ApplicationException("Trace 2"); }
                catch (Exception e) { _logger?.LogTrace(e); }
                _logger?.LogTrace(() => $"Trace {ph}");

                _logger?.LogDebug("Debug 1");
                try { throw new ApplicationException("Debug 2"); }
                catch (Exception e) { _logger?.LogDebug(e); }
                _logger?.LogDebug(() => $"Debug {ph}");

                _logger?.LogInfo("Info 1");
                try { throw new ApplicationException("Info 2"); }
                catch (Exception e) { _logger?.LogInfo(e); }
                _logger?.LogInfo(() => $"Info {ph}");

                _logger?.LogWarning("Warning 1");
                try { throw new ApplicationException("Warning 2"); }
                catch (Exception e) { _logger?.LogWarning(e); }
                _logger?.LogWarning(() => $"Warning {ph}");

                _logger?.LogError("Error 1");
                try { throw new ApplicationException("Error 2"); }
                catch (Exception e) { _logger?.LogError(e); }
                _logger?.LogError(() => $"Error {ph}");

                _logger?.LogCritical("Critical 1");
                try { throw new ApplicationException("Critical 2"); }
                catch (Exception e) { _logger?.LogCritical(e); }
                _logger?.LogCritical(() => $"Critical {ph}");
            }
            catch (Exception e)
            {
                WriteError(e);
            }
            if(String.IsNullOrWhiteSpace(param))
            {
                _sectionTime.Stop();
                WriteInfo($"<- Write basic logs {_sectionTime.ElapsedMilliseconds}ms");
            }
        }

        private void BasicLogs_90000()
        {
            try
            {
                WriteInfo("-> Write 90000 basic logs");
                _sectionTime.Start();
                for(int i = 0; i < 5000; i++)
                {
                    BasicLogs(i.ToString());
                }
            }
            catch(Exception e)
            {
                WriteError(e);
            }
            _sectionTime.Stop();
            WriteInfo($"<- Write 90000 basic logs {_sectionTime.ElapsedMilliseconds}ms");
        }

        private async Task BasicLogs_5_Threads_90000()
        {
            try
            {
                WriteInfo("-> Write 90000 basic logs with 5 threads");
                _sectionTime.Start();

                ConcurrentBag<long> testTimes = new ConcurrentBag<long>();
                List<Task> tasks = new List<Task>();
                for(int n = 1; n <= 5; n++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        int tld = Thread.CurrentThread.ManagedThreadId;
                        WriteInfo($"Thread {tld} started");
                        var sw = new Stopwatch();
                        sw.Start();
                        for(int i = 0; i < 1000; i++)
                            BasicLogs($"Task {tld}-{i}");
                        sw.Stop();
                        var t = sw.ElapsedMilliseconds;
                        testTimes.Add(t);
                        WriteInfo($"Task {tld} stopped. 1000 logs in {t}ms");
                    }));

                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
                long time = 0;
                while(testTimes.TryTake(out var res))
                {
                    time += res;
                }
                
                WriteInfo($"<- Test Write 90000 basic logs with 5 threads {time}ms");
            }
            catch(Exception e)
            {
                WriteError(e);
            }
            _sectionTime.Stop();
            WriteInfo($"<- Test 'Write 90000 basic logs with 5 threads' closed");
        }

        private void BasicLogs_90000_Delayed()
        {
            try
            {
                WriteInfo("-> Write 180000 basic logs");
                _sectionTime.Start();
                for(int i = 0; i < 10000; i++)
                {
                    BasicLogs(i.ToString());
                    Thread.Sleep(5);
                }
            }
            catch(Exception e)
            {
                WriteError(e);
            }
            _sectionTime.Stop();
            WriteInfo($"<- Write 90000 basic logs {_sectionTime.ElapsedMilliseconds}ms");
        }

        private void WriteInfo(object obj)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine($"### {obj}");

            Console.ForegroundColor = original;
        }

        private void WriteError(object obj)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(obj);

            Console.ForegroundColor = original;
        }

        private void WriteStartingHeader()
        {
            _completeTime.Start();
            var original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Start the Rlyeh.Logger test {DateTime.Now:MM-dd-yyyy_HH-mm-ss}");
            Console.WriteLine($"==============================================={Environment.NewLine}");
            Console.ForegroundColor = original;
        }

        private void WriteFinishHeader()
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            _completeTime.Stop();
            Console.WriteLine($"### {_completeTime.ElapsedMilliseconds}ms expired{Environment.NewLine}");
            Console.WriteLine("\t+-------------+");
            Console.WriteLine("\t|   THE END   |");
            Console.WriteLine($"\t+-------------+{Environment.NewLine}");
            Console.ForegroundColor = original;
        }
    }
}