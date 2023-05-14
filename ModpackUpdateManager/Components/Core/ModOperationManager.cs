using ModpackUpdateManager.Enums;
using ModpackUpdateManager.Models;
using ModpackUpdateManager.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Components
{
    public partial class Core
    {
        //Make private
        private class ModOperationManager
        {
            private string SearchUrl;

            private CallbackManager callbackManager;
            private UserMessaging userMessaging;
            private ModDataAccessor modDataAccessor;
            private Automation automation;
            private LifeCycleManager lifeCycleManager;
            private Miscellaneous miscellaneous;

            private string invalidDownloadsFolderName;
            private string reportModPath;
            private string dependenciesModPath;

            private static string targetedModFileName;

            private Dictionary<string, string> gameVersionIds = new Dictionary<string, string>();
            private Dictionary<string, string> gameFlavorIds = new Dictionary<string, string>();
            private Dictionary<ModCompletionStatus, List<LogObject>> report = new Dictionary<ModCompletionStatus, List<LogObject>>();

            private volatile string downloadedFileName = "";

            public ModOperationManager(string _reportModPath, string getModSearchResultFullPath, string _invalidDownloadsFolderName, string _dependenciesModPath,
                Dictionary<string, string> _gameVersionIds, Dictionary<string, string> _gameFlavorIds, List<string> searchTermBlacklist, UserMessaging _userMessaging)
            {
                foreach (ModCompletionStatus value in Enum.GetValues(typeof(ModCompletionStatus)))
                {
                    report[value] = new List<LogObject>();
                }

                userMessaging = _userMessaging;
                reportModPath = _reportModPath;
                invalidDownloadsFolderName = _invalidDownloadsFolderName;
                dependenciesModPath = _dependenciesModPath;
                gameFlavorIds = _gameFlavorIds;
                gameVersionIds = _gameVersionIds;
                PersistentVariables.SetCurrentModListIndex(0);
                PersistentVariables.SetIsInAutoMode(false);
                PersistentVariables.SetIsTaskCancelled(false);

                callbackManager = new CallbackManager(_userMessaging, this);
                modDataAccessor = new ModDataAccessor(gameFlavorIds, searchTermBlacklist, callbackManager.onModTomlFileNotFound, callbackManager.OnSkippingExistingMods);

                automation = new Automation(gameFlavorIds, modDataAccessor.GetNonDesiredApis(), getModSearchResultFullPath, modDataAccessor);

                miscellaneous = new Miscellaneous(userMessaging, modDataAccessor, reportModPath, report);
                lifeCycleManager = new LifeCycleManager(this, automation, userMessaging, modDataAccessor, miscellaneous);
            }

            public async Task ManualStep()
            {
                await lifeCycleManager.ManageManualModeLifecycle();
            }

            public async Task StartAutoMode()
            {
                await lifeCycleManager.ManageAutoModeLifecycle();
            }

            public void InitializeOperations(Form1 form1)
            {
                ModData ModData = modDataAccessor.GetCurrentlyProcessedModData();

                if (ModData == null)
                {
                    System.Windows.Forms.MessageBox.Show($"No mods found in {PersistentVariables.GetSourceModPath()}. Exiting...");
                    form1.Close();
                    return;
                }

                SearchUrl = Utilities.BuildCurseForgeSearchUrl(ModData.searchableName);
                BrowserManager.Initialize(SearchUrl, form1, callbackManager.OnDownloadUpdated, callbackManager.ParseDataFromRequestUrl);

                miscellaneous.ShowSearchingUserMessage();
            }

            public async Task SkipMod(ModCompletionStatus status, string reason)
            {
                string modName = modDataAccessor.GetCurrentlyProcessedModData().displayName;
                userMessaging.ShowMessage($"Skipping mod {modName}...");

                miscellaneous.ManageReport(status, modName, reason);

                if (!TryGetNextModInList())
                {
                    FinalizeUpdateProcess();
                }

                await miscellaneous.LoadSearchUrl();
            }

            public async Task LoadSearchUrl()
            {
                await miscellaneous.LoadSearchUrl();
            }

            public string GetTargetedModFileName()
            {
                return targetedModFileName;
            }

            #region Private Methods

            //Look into releaseType. 1 is for release. 2 for beta, maybe 3 for alpha?
            private async Task<TaskResult> TryProcessMod()
            {
                ModDataApiResponse modDataApiResponse = await GetModForTargetVersion();
                string modFileName = modDataApiResponse.data != null ? modDataApiResponse.data[0].fileName : "";

                targetedModFileName = modFileName;

                if (PersistentVariables.GetIsTaskCancelled())
                {
                    return TaskResult.Cancelled;
                }

                if (String.IsNullOrEmpty(targetedModFileName))
                {
                    string downloadFileNotFoundMessage = $"Could not find file for mod {modDataAccessor.GetCurrentlyProcessedModData().displayName} available for the target Minecraft version.";
                    userMessaging.ShowMessage(downloadFileNotFoundMessage);
                    return TaskResult.Failure;
                }

                if (PersistentVariables.GetIsTaskCancelled())
                {
                    return TaskResult.Cancelled;
                }

                userMessaging.ShowMessage($"Successfully found mod file {modDataAccessor.GetCurrentlyProcessedModData().displayName} for target Minecraft version.");

                if (PersistentVariables.GetDownloadMods())
                {
                    userMessaging.ShowMessage($"Downloading File {targetedModFileName}...");
                    await BrowserManager.LoadUrl(string.Format(AppSettings.GetDownloadUrl(), PersistentVariables.GetBaseModAddress(), modDataApiResponse.data[0].id));
                }

                return TaskResult.Success;
            }

            public void SetDownloadedFileName(string _downloadedFileName)
            {
                downloadedFileName = _downloadedFileName;
            }

            public async Task<TaskResult> TryProcessModForTargetedVersion()
            {
                TaskResult hasProcessedMod = await TryProcessMod();

                if (hasProcessedMod == TaskResult.Failure)
                {
                    string message = "Could not find corresponding file for the target Minecraft version";

                    await SkipMod(ModCompletionStatus.Failure, message);
                    return TaskResult.Failure;
                }
                else if (hasProcessedMod == TaskResult.Cancelled)
                    return TaskResult.Cancelled;

                string modName = modDataAccessor.GetCurrentlyProcessedModData().displayName;

                if (PersistentVariables.GetDownloadMods())
                {
                    string fileName = await GetDownloadedFileName();

                    if (!ValidateDownloadedMod(fileName))
                    {
                        string message = $"File {fileName} not matching with source. Moved to invalid, please manually confirm.";

                        userMessaging.ShowMessage(message);
                        miscellaneous.ManageReport(ModCompletionStatus.Flagged, modName, message);
                    }
                    else
                    {
                        miscellaneous.ManageReport(ModCompletionStatus.Success, modName, "File downloaded successfully.");
                    }
                }
                else
                {
                    miscellaneous.ManageReport(ModCompletionStatus.Success, modName, "Successfully found mod for target version.");
                }

                if (!TryGetNextModInList())
                {
                    FinalizeUpdateProcess();
                }

                return TaskResult.Success;
            }

            private bool TryGetNextModInList()
            {
                int dataIndex = PersistentVariables.GetCurrentModListIndex();

                PersistentVariables.SetCurrentModListIndex(++dataIndex);

                if (dataIndex >= modDataAccessor.GetSourceModQuantity())
                {
                    return false;
                }

                return true;
            }

            private async Task<ModDataApiResponse> GetModForTargetVersion()
            {
                userMessaging.ShowMessage($"Initiating version availability verification...");

                string baseModAddress = BrowserManager.Address;
                PersistentVariables.SetBaseModAddress(baseModAddress);

                userMessaging.ShowMessage($"Loading primary url...");

                if (PersistentVariables.GetIsTaskCancelled())
                {
                    return null;
                }

                //Load this url which triggers Network requests that divulge the modId
                await BrowserManager.LoadUrl($"{baseModAddress}/files?pageIndex=0&pageSize=20&sort=dateCreated&sortDescending=true&removeAlphas=true");

                if (PersistentVariables.GetIsTaskCancelled())
                {
                    return null;
                }

                //The modId is saved persistently when intercepted
                string currentModId = PersistentVariables.GetCurrentModId();

                userMessaging.ShowMessage($"Verifying latest file mod available...");
                //Build url to search for latest version of the mod matching the desired API and Minecraft version
                string ModDataUrl = String.Format(AppSettings.GetCurseforgeApiMinecraftSearchUrl(), currentModId, gameVersionIds[PersistentVariables.GetSelectedVersion()], gameFlavorIds[PersistentVariables.GetSelectedApi()]);

                ModDataApiResponse modDataApiResponse = JsonConvert.DeserializeObject<ModDataApiResponse>(BrowserManager.GetRequest(ModDataUrl));

                return modDataApiResponse;
            }

            private async Task<string> GetDownloadedFileName()
            {
                // Uses downloadedFileName as a flag lifted by the OnTargetedModDownloadCompleted
                // Reads the flag value, outputs it but resets the flag
                await Task.Run(() =>
                {
                    while (String.IsNullOrEmpty(downloadedFileName))
                    {
                        Thread.Sleep(200);
                    }
                });

                string result = downloadedFileName;

                downloadedFileName = "";

                return result;
            }


            private bool ValidateDownloadedMod(string fileName)
            {
                string outputPath = Path.Combine(PersistentVariables.GetOutputModPath(), fileName);
                string invalidFolderPath = Path.Combine(PersistentVariables.GetOutputModPath(), invalidDownloadsFolderName);

                if (!string.IsNullOrEmpty(fileName))
                {
                    if (!miscellaneous.IsDownloadedModDisplayNameMatchingSource(modDataAccessor.GetCurrentlyProcessedModData().fileName, fileName))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(invalidFolderPath);
                        string destinationFullPath = Path.Combine(invalidFolderPath, fileName);

                        if (File.Exists(destinationFullPath))
                        {
                            File.Delete(destinationFullPath);
                        }

                        File.Move(outputPath, destinationFullPath);

                        return false;
                    }
                }

                return true;
            }

            private void FinalizeUpdateProcess()
            {
                HashSet<string> requiredDependencies = modDataAccessor.GetAllOutputDependencies();

                if (requiredDependencies != null)
                {
                    foreach (string dependency in requiredDependencies)
                    {
                        userMessaging.ShowMessage($"Dependency {dependency} required for modpack, please confirm presence manually.");
                    }

                    JsonFileHandler.SerializeJsonFile<HashSet<string>>(dependenciesModPath, requiredDependencies, false);
                }

                userMessaging.ShowMessageBox("Modpack update process completed, please verify log files in output folder.", "Operation Successfully Completed");
            }

            #endregion
        }
    }
}
