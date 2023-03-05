using ModpackUpdateManager.Utils;

namespace ModpackUpdateManager.Components
{
    public partial class Core
    {
        private class CallbackManager
        {
            UserMessaging userMessaging;
            ModOperationManager modOperationManager;

            public CallbackManager(UserMessaging _userMessaging, ModOperationManager _modOperationManager)
            {
                userMessaging = _userMessaging;
                modOperationManager = _modOperationManager;
            }

            public void onModTomlFileNotFound(string filePath)
            {
                userMessaging.ShowMessage($"Warning: Could not file mods.toml file in {filePath}");
            }

            public void OnTargetedModDownloadCompleted(string fileName)
            {
                userMessaging.ShowMessage($"File {fileName} downloaded successfully...");
                modOperationManager.SetDownloadedFileName(fileName);
            }

            public void OnOtherDownloadCompleted(string filename)
            {
                userMessaging.ShowMessage($"Non modpack file {filename} manually downloaded successfully...");
            }

            public CefSharp.IResourceRequestHandler ParseDataFromRequestUrl(CefSharp.IRequest request)
            {
                if (request.Url.Contains(AppSettings.GetApiBaseUrl()))
                {
                    PersistentVariables.SetCurrentModId(request.Url.Split('/')[6]);
                }
                else if (request.Url.Contains(AppSettings.GetBaseModUrl()))
                {
                    //It's assumed that we are on a mod's main page
                    if (!PersistentVariables.GetIsInAutoMode() && Utilities.CountCharsInString(request.Url, '/') == 5)
                    {
                        userMessaging.ShowMessage("Please press the Manual Step button to download this mod...");
                    }
                }

                return null;
            }

            public void OnDownloadUpdated(CefSharp.DownloadItem downloadItem)
            {
                string targetedModFileName = modOperationManager.GetTargetedModFileName();
                // Check if downloadItem matches the mod targeted by the system or if it something downloaded by the user
                if (downloadItem.IsComplete && targetedModFileName != null && System.IO.Path.GetFileName(downloadItem.FullPath) == Utilities.FastReplace(targetedModFileName, " ", "+"))
                {
                    OnTargetedModDownloadCompleted(System.IO.Path.GetFileName(downloadItem.FullPath));
                }
                else if (downloadItem.IsComplete)
                {
                    //Manually added
                    OnOtherDownloadCompleted(System.IO.Path.GetFileName(downloadItem.FullPath));
                }
                else
                {
                    userMessaging.ShowMessage("Downloading...");
                }
            }

            public void OnSkippingExistingMods()
            {
                userMessaging.ShowMessage("Found files in output folder, will skip them as specified in configuration");
            }

        }
    }
}
