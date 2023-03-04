using ModpackUpdateManager.Enums;
using ModpackUpdateManager.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Managers
{
    public class ModOperationManager
    {
        private string SearchUrl;

        private UserMessaging userMessaging;
        private ModDataAccessor modDataAccessor;
        private Automation automation;

        private const string invalidDownloadsFolderName = "invalid";
        private const string logFolderName = "logs";

        private const string executionLogFileName = "execution.log";
        private const string reportLogFileName = "report.log";
        private const string dependenciesLogFileName = "dependencies.log";

        private string reportModPath = Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName, reportLogFileName);
        private string dependenciesModPath = Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName, dependenciesLogFileName);
        private string executionLogFullPath = Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName, executionLogFileName);

        private static string targetedModFileName = "";

        private Dictionary<string, string> gameVersionIds = new Dictionary<string, string>();
        private Dictionary<string, string> gameFlavorIds = new Dictionary<string, string>();
        private Dictionary<ModCompletionStatus, List<LogObject>> report = new Dictionary<ModCompletionStatus, List<LogObject>>();

        private volatile bool isProcessing = false;
        private volatile bool isStopping = false;

        private volatile string downloadedFileName = "";

        public ModOperationManager(Form1 MainForm, string getModSearchResultFullPath, Dictionary<string, string> _gameVersionIds, Dictionary<string, string> _gameFlavorIds, List<string> _searchTermBlacklist)
        {
            foreach (ModCompletionStatus value in Enum.GetValues(typeof(ModCompletionStatus)))
            {
                report[value] = new List<LogObject>();
            }

            InitializeOutputFiles();

            gameFlavorIds = _gameFlavorIds;
            gameVersionIds = _gameVersionIds;
            PersistentVariables.SetCurrentModListIndex(0);
            PersistentVariables.SetIsInAutoMode(false);
            PersistentVariables.SetIsTaskCancelled(false);
            userMessaging = new UserMessaging(MainForm);
            modDataAccessor = new ModDataAccessor(gameFlavorIds, _searchTermBlacklist, onModTomlFileNotFound, OnSkippingExistingMods);
            automation = new Automation(gameFlavorIds, modDataAccessor.GetNonDesiredApis(), getModSearchResultFullPath, modDataAccessor);
            ModData ModData = modDataAccessor.GetCurrentlyProcessedModData();

            if (ModData == null)
            {
                System.Windows.Forms.MessageBox.Show($"No mods found in {PersistentVariables.GetSourceModPath()}. Exiting...");
                MainForm.Close();
                return;
            }

            SearchUrl = Utilities.BuildCurseForgeSearchUrl(ModData.searchableName);
            BrowserManager.Initialize(SearchUrl, this, MainForm, OnDownloadUpdated, ParseDataFromRequestUrl);

            ShowSearchingUserMessage();
        }
        #region Public Methods
        public async Task SkipMod(ModCompletionStatus status, string reason)
        {
            string modName = modDataAccessor.GetCurrentlyProcessedModData().displayName;
            userMessaging.ShowMessage($"Skipping mod {modName}...");

            ManageReport(status, modName, reason);

            if (!TryGetNextModInList())
            {
                FinalizeUpdateProcess();
            }

            await LoadSearchUrl();
        }

        public async void ManualStep()
        {
            if (!isProcessing)
            {
                isProcessing = true;
                PersistentVariables.SetIsInAutoMode(false);
                PersistentVariables.SetIsTaskCancelled(false);

                await ManageManualModeLifecycle();

                isProcessing = false;
            }
        }

        public async void Cancel()
        {
            if (!isStopping)
            {
                isStopping = true;
                userMessaging.ShowMessage("Cancelling task...");

                PersistentVariables.SetIsInAutoMode(false);
                PersistentVariables.SetIsTaskCancelled(true);
                // Give time for other tasks to close in the worst case scenario
                await Task.Delay(2100);
                PersistentVariables.SetIsTaskCancelled(false);
                await LoadSearchUrl();
                isStopping = false;
            }
        }

        public async void StartAutoMode()
        {
            if (!isProcessing)
            {
                isProcessing = true;

                PersistentVariables.SetIsInAutoMode(true);
                PersistentVariables.SetIsTaskCancelled(false);

                await ManageAutoModeLifecycle();

                isProcessing = false;
            }
        }

        public void Dispose()
        {
            BrowserManager.Dispose();
        }

        #endregion

        #region Private Methods

        private async Task InitializeOutputFiles()
        {
            Directory.CreateDirectory(Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName));
            await JsonFileHandler.CreateFile(reportModPath);
            await JsonFileHandler.CreateFile(dependenciesModPath);
            await JsonFileHandler.CreateFile(executionLogFullPath);
            LogFile.Initialize(executionLogFullPath);
        }
        //Look into releaseType. 1 is for release. 2 for beta, maybe 3 for alpha?
        private async Task<TaskResult> TryProcessMod()
        {
            ModDataApiResponse modDataApiResponse = await GetModForTargetVersion();
            string ModFileName = modDataApiResponse.data.Count > 0 ? modDataApiResponse.data[0].fileName : "";

            SetTargetedModFileName(ModFileName);

            if (PersistentVariables.GetIsTaskCancelled())
            {
                return TaskResult.Cancelled;
            }

            if (String.IsNullOrEmpty(ModFileName))
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
                userMessaging.ShowMessage($"Downloading File {ModFileName}...");
                await BrowserManager.LoadUrl(string.Format(AppSettings.GetDownloadUrl(), PersistentVariables.GetBaseModAddress(), modDataApiResponse.data[0].id));
            }

            return TaskResult.Success;
        }

        private async Task LoadSearchUrl()
        {
            ModData ModData = modDataAccessor.GetCurrentlyProcessedModData();
            string searchTerm = ModData.searchableName;

            string searchUrl = Utilities.BuildCurseForgeSearchUrl(searchTerm);
            ShowSearchingUserMessage();
            await BrowserManager.LoadUrl(searchUrl);
        }

        private async Task ManageAutoModeLifecycle()
        {
            await LoadSearchUrl();

            do
            {
                if (!PersistentVariables.GetIsInAutoMode() || PersistentVariables.GetIsTaskCancelled())
                    return;

                ModSearchResult modSearchResult = await automation.TrySelectTargetModFromSearchList();

                if (modSearchResult == null)
                {
                    await SkipMod(ModCompletionStatus.Failure, "Search yielded no valid results, could not automatically download.");
                    continue;
                }

                userMessaging.ShowMessage($"{modSearchResult.Value} automatically selected");

                if (!PersistentVariables.GetIsInAutoMode() || PersistentVariables.GetIsTaskCancelled())
                    return;

                TaskResult result = await TryProcessModForTargetedVersion();

                if (result == TaskResult.Cancelled)
                {
                    return;
                }
                else if (result == TaskResult.Failure)
                {
                    continue;
                }

                if (PersistentVariables.GetCurrentModListIndex() >= modDataAccessor.GetSourceModQuantity())
                {
                    break;
                }

                await LoadSearchUrl();

            } while (PersistentVariables.GetIsInAutoMode() && !PersistentVariables.GetIsTaskCancelled());

            PersistentVariables.SetIsInAutoMode(false);
        }

        private async Task ManageManualModeLifecycle()
        {
            if (IsOnModMainPage())
            {
                await TryProcessModForTargetedVersion();
            }

            await LoadSearchUrl();
        }

        private async Task<TaskResult> TryProcessModForTargetedVersion()
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
                    ManageReport(ModCompletionStatus.Flagged, modName, message);
                }
                else
                {
                    ManageReport(ModCompletionStatus.Success, modName, "File downloaded successfully.");
                }
            }
            else
            {
                ManageReport(ModCompletionStatus.Success, modName, "Successfully found mod for target version.");
            }

            if (!TryGetNextModInList())
            {
                FinalizeUpdateProcess();
            }

            return TaskResult.Success;
        }

        private void ManageReport(ModCompletionStatus status, string modName, string message)
        {
            report[status].Add(new LogObject(modName, message));
            JsonFileHandler.SerializeJsonFile<Dictionary<ModCompletionStatus, List<LogObject>>>(reportModPath, report, false);
        }

        private bool IsOnModMainPage()
        {
            if (BrowserManager.Address.Contains(AppSettings.GetBaseModUrl()))
            {
                //It's assumed that we are on a mod's main page
                return (Utilities.CountCharsInString(BrowserManager.Address, '/') == 5);
            }
            return false;
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
            await BrowserManager.LoadUrl($"{baseModAddress}/files/all?page=1");

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

        private void OnDownloadUpdated(CefSharp.DownloadItem downloadItem)
        {
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

        private void OnSkippingExistingMods()
        {
            userMessaging.ShowMessage("Found files in output folder, will skip them as specified in configuration");
        }

        private void ShowSearchingUserMessage()
        {
            userMessaging.ShowMessage($"Searching for {modDataAccessor.GetCurrentlyProcessedModData().searchableName}...");
            if (!PersistentVariables.GetIsInAutoMode())
            {
                userMessaging.ShowMessage($"Please manually select the appropriate mod from the results the click the Step button again.");
            }
        }

        private bool ValidateDownloadedMod(string fileName)
        {
            string outputPath = Path.Combine(PersistentVariables.GetOutputModPath(), fileName);
            string invalidFolderPath = Path.Combine(PersistentVariables.GetOutputModPath(), invalidDownloadsFolderName);

            if (!string.IsNullOrEmpty(fileName))
            {
                if (!IsDownloadedModDisplayNameMatchingSource(modDataAccessor.GetCurrentlyProcessedModData().fileName, fileName))
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

        private bool IsDownloadedModDisplayNameMatchingSource(string sourceFileName, string targetFileName)
        {
            string sourcePath = PersistentVariables.GetSourceModPath();
            string targetPath = PersistentVariables.GetOutputModPath();

            ModData sourceModData = modDataAccessor.GetModData(sourcePath, sourceFileName);
            ModData targetModData = modDataAccessor.GetModData(targetPath, targetFileName);

            return sourceModData.displayName == targetModData.displayName;
        }

        private void onModTomlFileNotFound(string filePath)
        {
            userMessaging.ShowMessage($"Warning: Could not file mods.toml file in {filePath}");
        }

        private void OnTargetedModDownloadCompleted(string fileName)
        {
            userMessaging.ShowMessage($"File {fileName} downloaded successfully...");
            downloadedFileName = fileName;
        }

        private void OnOtherDownloadCompleted(string filename)
        {
            userMessaging.ShowMessage($"Non modpack file {filename} manually downloaded successfully...");
        }

        private CefSharp.IResourceRequestHandler ParseDataFromRequestUrl(CefSharp.IRequest request)
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

        private static void SetTargetedModFileName(string _targetedModFileName)
        {
            targetedModFileName = _targetedModFileName;
        }
        #endregion
    }
}
