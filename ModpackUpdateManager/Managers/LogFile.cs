using System;

namespace ModpackUpdateManager.Managers
{
    public static class LogFile
    {
        private static string logFilePath = @".\\logs.log";
        public static void Initialize(bool clearLogFile = true)
        {
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
