﻿using ModpackUpdateManager.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Managers
{
    public class ModOperationManager
    {
        private string SearchUrl;

        private UserMessaging userMessaging;
        private ModDataAccessor modDataAccessor;
        private Automation automation;

        private const string logFolderName = "logs";
        private const string executionLogFileName = "execution.log";
        private const string missingModFileName = "missing.log";
        private const string dependenciesLogFileName = "dependencies.log";
        private const string manuallyAddedModFileName = "manuallyAdded.log";
        private const string invalidDownloadsFolderName = "invalid";

        private string missingModPath = Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName, missingModFileName);
        private string dependenciesModPath = Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName, dependenciesLogFileName);
        private string manuallyAddedModPath = Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName, manuallyAddedModFileName);
        private string executionLogFullPath = Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName, executionLogFileName);

        private static string lastProcessedModFileName;

        private Dictionary<string, string> gameVersionIds = new Dictionary<string, string>();
        private Dictionary<string, string> gameFlavorIds = new Dictionary<string, string>();

        private bool isProcessing = false;

        public ModOperationManager(Form1 MainForm, string getModSearchResultFullPath, Dictionary<string, string> _gameVersionIds, Dictionary<string, string> _gameFlavorIds, List<string> _searchTermBlacklist)
        {
            gameFlavorIds = _gameFlavorIds;
            gameVersionIds = _gameVersionIds;
            Directory.CreateDirectory(Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName));
            LogFile.Initialize(executionLogFullPath);
            PersistentVariables.SetCurrentModListIndex(0);
            PersistentVariables.SetIsInAutoMode(false);
            PersistentVariables.SetIsAutoModePaused(false);
            userMessaging = new UserMessaging(MainForm);
            modDataAccessor = new ModDataAccessor(gameFlavorIds, _searchTermBlacklist, onModTomlFileNotFound, OnSkippingExistingMods);
            automation = new Automation(gameFlavorIds, modDataAccessor.GetNonDesiredApis(), getModSearchResultFullPath, this);
            ModData ModData = modDataAccessor.GetCurrentlyProcessedModData();

            if (ModData == null)
            {
                System.Windows.Forms.MessageBox.Show($"No mods found in {PersistentVariables.GetSourceModPath()}. Exiting...");
                MainForm.Close();
                return;
            }

            SearchUrl = BuildSearchUrl(ModData.searchableName);
            BrowserManager.Initialize(SearchUrl, this, MainForm, OnDownloadUpdated, ParseDataFromRequestUrl);

            ShowSearchingUserMessage();
        }

        //Look into releaseType. 1 is for release. 2 for beta, maybe 3 for alpha?
        public async Task<int> DownloadMod()
        {
            userMessaging.ShowMessage($"Initiating Download Sequence...");

            string baseModAddress = BrowserManager.Address;
            PersistentVariables.SetBaseModAddress(baseModAddress);

            userMessaging.ShowMessage($"Loading primary url...");

            if (PersistentVariables.GetIsAutoModePaused())
            {
                return 1;
            }

            //Load this url which triggers Network requests that divulge the modId
            await BrowserManager.LoadUrl($"{baseModAddress}/files/all?page=1");

            if (PersistentVariables.GetIsAutoModePaused())
            {
                return 1;
            }

            //The modId is saved persistently when intercepted
            string currentModId = PersistentVariables.GetCurrentModId();

            userMessaging.ShowMessage($"Verifying latest file mod available...");
            //Build url to search for latest version of the mod matching the desired API and Minecraft version
            string ModDataUrl = String.Format(AppSettings.GetCurseforgeApiMinecraftSearchUrl(), currentModId, gameVersionIds[PersistentVariables.GetSelectedVersion()], gameFlavorIds[PersistentVariables.GetSelectedApi()]);

            ModDataApiResponse modDataApiResponse = JsonConvert.DeserializeObject<ModDataApiResponse>(BrowserManager.GetRequest(ModDataUrl));

            string ModFileName = modDataApiResponse.data.Count > 0 ? modDataApiResponse.data[0].fileName : "";

            if (String.IsNullOrEmpty(ModFileName))
            {
                string response = JsonConvert.SerializeObject(modDataApiResponse);
                string downloadFileNotFoundMessage = $"Could not find file for mod {modDataAccessor.GetCurrentlyProcessedModData().displayName} available for this Minecraft version";
                userMessaging.ShowMessage(downloadFileNotFoundMessage);
                SkipMod(downloadFileNotFoundMessage);
                return -1;
            }

            if (PersistentVariables.GetIsAutoModePaused())
            {
                return 1;
            }

            SetLastProcessedModFileName(ModFileName);
            userMessaging.ShowMessage($"Successfully found mod file to download for mod {modDataAccessor.GetCurrentlyProcessedModData().displayName}");
            userMessaging.ShowMessage($"Downloading File {ModFileName}...");
            await BrowserManager.LoadUrl(string.Format(AppSettings.GetDownloadUrl(), baseModAddress, modDataApiResponse.data[0].id));

            return 0;
        }

        public void SkipMod(string reason)
        {
            string modName = modDataAccessor.GetCurrentlyProcessedModData().displayName;
            userMessaging.ShowMessage($"Skipping mod {modName}...");
            SaveToJson(missingModPath, modName, reason, false);
            FinalizeDownload();
        }

        public async Task<int> LoadSearchUrl()
        {
            ModData ModData = modDataAccessor.GetCurrentlyProcessedModData();
            string searchTerm = ModData.searchableName;

            string searchUrl = BuildSearchUrl(searchTerm);
            ShowSearchingUserMessage();
            await BrowserManager.LoadUrl(searchUrl);

            if (PersistentVariables.GetIsInAutoMode() && !PersistentVariables.GetIsAutoModePaused())
            {
                await automation.SelectAndDownloadMod();
            }

            return 0;
        }

        public ModSearchResult OnSearchResultAutoSelected(ModSearchResult modSearchResult)
        {
            userMessaging.ShowMessage($"{modSearchResult.Value} automatically selected");
            return modSearchResult;
        }

        public async void ManualStep()
        {
            if (!isProcessing)
            {
                isProcessing = true;
                PersistentVariables.SetIsInAutoMode(false);
                PersistentVariables.SetIsAutoModePaused(false);
                if (IsOnModMainPage())
                {
                    await DownloadMod();
                }
                else
                {
                    ShowSearchingUserMessage();
                    await LoadSearchUrl();
                }
                isProcessing = false;
            }
        }

        public async void StopAutoMod()
        {
            if (!isProcessing && PersistentVariables.GetIsInAutoMode())
            {
                isProcessing = true;
                userMessaging.ShowMessage("Pausing Auto Mode...");

                PersistentVariables.SetIsInAutoMode(false);
                PersistentVariables.SetIsAutoModePaused(true);
                // Give time for other tasks to close in the worst case scenario
                await Task.Delay(2100);
                PersistentVariables.SetIsAutoModePaused(false);
                await LoadSearchUrl();
                isProcessing = false;
            }
        }

        public async void StartAutoMode()
        {
            if (!isProcessing)
            {
                isProcessing = true;
                PersistentVariables.SetIsInAutoMode(true);
                PersistentVariables.SetIsAutoModePaused(false);
                await LoadSearchUrl();
                isProcessing = false;
            }
        }

        public string BuildSearchUrl(string modName)
        {
            modName = System.Web.HttpUtility.UrlEncode(modName);
            return $"{AppSettings.GetCurseforgeMinecraftSearchUrl()}&search={modName}";
        }

        public ModDataAccessor GetModDataAccessor()
        {
            return modDataAccessor;
        }

        public void Dispose()
        {
            BrowserManager.Dispose();
        }
        public bool IsOnModMainPage()
        {
            if (BrowserManager.Address.Contains(AppSettings.GetBaseModUrl()))
            {
                //It's assumed that we are on a mod's main page
                return (Utilities.CountCharsInString(BrowserManager.Address, '/') == 5);
            }
            return false;
        }

        private void OnDownloadUpdated(CefSharp.DownloadItem downloadItem)
        {
            if (downloadItem.IsComplete && lastProcessedModFileName != null && System.IO.Path.GetFileName(downloadItem.FullPath) == Utilities.FastReplace(lastProcessedModFileName, " ", "+"))
            {
                OnModFileDownloadCompleted(System.IO.Path.GetFileName(downloadItem.FullPath));
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

        private void SaveToJson(string path, string name, string reason, bool appendToFile = true)
        {
            JsonFileHandler.WriteJsonToFile<LogObject>(path, new LogObject(name, reason), appendToFile);
        }

        private void SaveToJson(string path, string name, bool appendToFile)
        {
            JsonFileHandler.WriteJsonToFile<string>(path, name, appendToFile);
        }

        private void ShowSearchingUserMessage()
        {
            userMessaging.ShowMessage($"Searching for {modDataAccessor.GetCurrentlyProcessedModData().searchableName}...");
            if (!PersistentVariables.GetIsInAutoMode())
            {
                userMessaging.ShowMessage($"Please manually select the appropriate mod from the results the click the Step button again.");
            }
        }
        private async Task FinalizeDownload(string fileName = "")
        {
            int dataIndex = PersistentVariables.GetCurrentModListIndex();
            string outputPath = Path.Combine(PersistentVariables.GetOutputModPath(), fileName);
            string invalidFolderPath = Path.Combine(PersistentVariables.GetOutputModPath(), invalidDownloadsFolderName);

            if (!string.IsNullOrEmpty(fileName))
            {
                if (!IsDownloadedModDisplayNameMatchingSource(modDataAccessor.GetCurrentlyProcessedModData().fileName, fileName))
                {
                    userMessaging.ShowMessage($"File {fileName} not matching with source. Moved to invalid, please manually confirm.");


                    DirectoryInfo di = Directory.CreateDirectory(invalidFolderPath);
                    string destinationFullPath = Path.Combine(invalidFolderPath, fileName);

                    if (File.Exists(destinationFullPath))
                    {
                        File.Delete(destinationFullPath);
                    }

                    File.Move(outputPath, destinationFullPath);
                }
            }

            PersistentVariables.SetCurrentModListIndex(++dataIndex);

            if (dataIndex < modDataAccessor.GetSourceModQuantity())
            {
                await LoadSearchUrl();
            }
            else
            {
                FinalizeUpdateProcess();
            }
        }

        private void FinalizeUpdateProcess()
        {
            HashSet<string> missingDependencies = modDataAccessor.GetAllOutputDependencies();
            int len = missingDependencies.Count;

            foreach (string dependency in missingDependencies)
            {
                userMessaging.ShowMessage($"Dependency {dependency} required for modpack, please confirm presence manually.");
                SaveToJson(dependenciesModPath, dependency, true);
            }
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

        private async Task OnModFileDownloadCompleted(string fileName)
        {
            userMessaging.ShowMessage($"File {fileName} downloaded successfully...");
            await FinalizeDownload(fileName);
        }

        private void OnOtherDownloadCompleted(string filename)
        {
            userMessaging.ShowMessage($"Non modpack file {filename} manually downloaded successfully...");
            SaveToJson(manuallyAddedModPath, filename, "Manually added");
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
        private static void SetLastProcessedModFileName(string _lastProcessedModFileName)
        {
            lastProcessedModFileName = _lastProcessedModFileName;
        }
    }
}
