using System.Configuration;

namespace ModpackUpdateManager.Managers
{
    public static class AppSettings
    {
        private static string curseforgeMinecraftSeachUrl = ConfigurationManager.AppSettings["curseforgeMinecraftSeachUrl"];
        private static string curseforgeApiMinecraftSeachUrl = ConfigurationManager.AppSettings["curseforgeApiMinecraftSeachUrl"];
        private static string apiBaseUrl = ConfigurationManager.AppSettings["apiBaseUrl"];
        private static string baseModUrl = ConfigurationManager.AppSettings["baseModUrl"];
        private static string downloadUrl = ConfigurationManager.AppSettings["downloadUrl"];

        public static string GetCurseforgeMinecraftSearchUrl()
        {
            return curseforgeMinecraftSeachUrl;
        }

        public static string GetCurseforgeApiMinecraftSearchUrl()
        {
            return curseforgeApiMinecraftSeachUrl;
        }

        public static string GetApiBaseUrl()
        {
            return apiBaseUrl;
        }

        public static string GetBaseModUrl()
        {
            return baseModUrl;
        }

        public static string GetDownloadUrl()
        {
            return downloadUrl;
        }

    }
}
