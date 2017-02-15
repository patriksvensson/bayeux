using System;

namespace Bayeux.Diagnostics
{
    public sealed class ConsoleLogger : IBayeuxLogger
    {
        private readonly object _lock;

        public ConsoleLogger()
        {
            _lock = new object();
        }

        public void Write(BayeuxLogLevel level, string format, params object[] args)
        {
            lock (_lock)
            {
                Console.ForegroundColor = GetColor(level);
                Console.WriteLine(format, args);
                Console.ResetColor();
            }
        }

        private ConsoleColor GetColor(BayeuxLogLevel level)
        {
            switch (level)
            {
                case BayeuxLogLevel.Warning:
                    return ConsoleColor.Yellow;
                case BayeuxLogLevel.Information:
                    return ConsoleColor.White;
                case BayeuxLogLevel.Debug:
                    return ConsoleColor.DarkGray;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}
