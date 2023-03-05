namespace ModpackUpdateManager.Utils
{
    public static class PersistentVariables
    {
        public static void SetCurrentModListIndex(int index)
        {
            Properties.Settings.Default.CurrentModListIndex = index;
            Properties.Settings.Default.Save();
        }

        public static int GetCurrentModListIndex()
        {
            return Properties.Settings.Default.CurrentModListIndex;
        }

        public static void SetBaseModAddress(string url)
        {
            Properties.Settings.Default.BaseModAddress = url;
            Properties.Settings.Default.Save();
        }

        public static string GetBaseModAddress()
        {
            return Properties.Settings.Default.BaseModAddress;
        }

        public static void SetCurrentModId(string modId)
        {
            Properties.Settings.Default.CurrentModId = modId;
            Properties.Settings.Default.Save();
        }

        public static string GetCurrentModId()
        {
            return Properties.Settings.Default.CurrentModId;
        }

        public static bool GetIsTaskCancelled()
        {
            return Properties.Settings.Default.IsTaskCancelled;
        }

        public static void SetIsTaskCancelled(bool isTaskCancelled)
        {
            Properties.Settings.Default.IsTaskCancelled = isTaskCancelled;
            Properties.Settings.Default.Save();
        }

        public static bool GetIsInAutoMode()
        {
            return Properties.Settings.Default.IsInAutoMode;
        }

        public static void SetIsInAutoMode(bool isInAutoMode)
        {
            Properties.Settings.Default.IsInAutoMode = isInAutoMode;
            Properties.Settings.Default.Save();
        }

        public static string GetSourceModPath()
        {
            return Properties.Settings.Default.SourceModPath;
        }

        public static void SetSourceModPath(string sourceModPath)
        {
            Properties.Settings.Default.SourceModPath = sourceModPath;
            Properties.Settings.Default.Save();
        }

        public static string GetOutputModPath()
        {
            return Properties.Settings.Default.OutputModPath;
        }

        public static void SetOutputModPath(string outputModPath)
        {
            Properties.Settings.Default.OutputModPath = outputModPath;
            Properties.Settings.Default.Save();
        }

        public static string GetSelectedVersion()
        {
            return Properties.Settings.Default.SelectedVersion;
        }

        public static void SetSelectedVersion(string selectedVersion)
        {
            Properties.Settings.Default.SelectedVersion = selectedVersion;
            Properties.Settings.Default.Save();
        }

        public static string GetSelectedApi()
        {
            return Properties.Settings.Default.SelectedApi;
        }

        public static void SetSelectedApi(string selectedApi)
        {
            Properties.Settings.Default.SelectedApi = selectedApi;
            Properties.Settings.Default.Save();
        }

        public static bool GetSkipExisting()
        {
            return Properties.Settings.Default.SkipExisting;
        }

        public static void SetSkipExisting(bool skipExisting)
        {
            Properties.Settings.Default.SkipExisting = skipExisting;
            Properties.Settings.Default.Save();
        }

        public static void SetDownloadMods(bool downloadMods)
        {
            Properties.Settings.Default.DownloadMods = downloadMods;
            Properties.Settings.Default.Save();
        }

        public static bool GetDownloadMods()
        {
            return Properties.Settings.Default.DownloadMods;
        }

    }
}
