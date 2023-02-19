using System;

namespace ModpackUpdateManager.Managers
{
    public static class LogFile
    {
        private static string logFilePath;
        public static void Initialize(string _logFilePath, bool clearLogFile = true)
        {
            logFilePath = _logFilePath;

            if (clearLogFile)
                ClearLogFile();
        }
        public static void LogMessage(string message)
        {
            Utilities.FileWriteAsync(logFilePath, $"{DateTime.Now.ToLongTimeString()}: {message}").Wait();
        }
        private static void ClearLogFile()
        {
            System.IO.File.WriteAllText(logFilePath, "");
        }
    }
}
