using NLog;

namespace cleddmessenger.Utils
{
    internal class LoggerUtility
    {
        // Create a logger instance
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // Log info message
        public static void LogInfo(string message)
        {
            Logger.Info(message);
        }

        // Log warning message
        public static void LogWarning(string message)
        {
            Logger.Warn(message);
        }

        // Log error message
        public static void LogError(string message)
        {
            Logger.Error(message);
        }

        // Log exception
        public static void LogError(Exception ex)
        {
            Logger.Error(ex, ex.Message);
        }
    }
}
