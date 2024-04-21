namespace Logger
{
    public static class LoggerFactory
    {
        public static ILogger GetLogger(bool debugLoggingEnable = false)
        {
            return new LoggerService()
            {
                EnableDebugLogging = debugLoggingEnable,
            };
        }

        private static ILogger? _nullLogger;
        public static ILogger NullLogger => _nullLogger ??= new NullLogger();
    }
}