using System;

namespace ModpackUpdateManager.Utils
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
        public static async void LogMessage(string message)
        {
            await Utilities.FileWriteAsync(logFilePath, $"{DateTime.Now.ToString("hh:mm:ss.fff")}: {message}", true);
        }
        private static void ClearLogFile()
        {
            System.IO.File.WriteAllText(logFilePath, "");
        }
    }
}
