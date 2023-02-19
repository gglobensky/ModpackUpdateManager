﻿using ModpackUpdateManager.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ModpackUpdateManager.Managers
{
    public class ModDataAccessor
    {
        public List<ModData> sourceFolderModData;
        private string sourceModFolderPath = PersistentVariables.GetSourceModPath();
        private string outputModFolderPath = PersistentVariables.GetOutputModPath();
        private List<string> nonDesiredApis = new List<string>();
        private List<string> searchTermBlacklist = new List<string>();

        public int GetSourceModQuantity()
        {
            return sourceFolderModData.Count;
        }

        public ModDataAccessor(Dictionary<string, string> gameFlavorIds, List<string> _searchTermBlacklist, Action<string> onSourceModTomlFileContentNotFound, Action onSkippingExisting)
        {
            InitializeData(gameFlavorIds);
            searchTermBlacklist = _searchTermBlacklist;
            sourceFolderModData = GetAllModData(sourceModFolderPath, onSourceModTomlFileContentNotFound);

            SkipExisting(onSkippingExisting);
        }
        public List<ModData> GetAllModData(string path, Action<string> onModTomlFileNotFound)
        {
            List<ModData> ModTomlFileContentDatum = new List<ModData>();
            string[] jarFiles = Directory.GetFiles(path, "*.jar")
                         .Select(Path.GetFileName)
                         .ToArray();

            foreach (string fileName in jarFiles)
            {
                ModData ModData = GetModData(path, fileName);

                if (ModData == null)
                {
                    onModTomlFileNotFound?.Invoke(Path.Combine(path, fileName));
                }
                else
                {
                    ModTomlFileContentDatum.Add(ModData);
                }
            }

            return ModTomlFileContentDatum;
        }

        public ModData GetModData(string path, string fileName)
        {
            ModData ModData = null;
            ModTomlFileContent ModTomlFileContent = JsonConvert.DeserializeObject<ModTomlFileContent>(ReadJarModTomlFileContent(Path.Combine(path, fileName)));

            if (ModTomlFileContent != null)
            {
                string displayName = "";
                string rawDisplayName = ModTomlFileContent.mods[0].displayName;

                if (rawDisplayName != null)
                {
                    displayName = Utilities.ToHumanReadable(rawDisplayName);
                    string searchableName = Utilities.FastReplace(displayName, PersistentVariables.GetSelectedApi(), " ", false, true);
                    searchableName = GetFormattedDisplayNameForSearch(searchableName);
                    ModData = new ModData(displayName, rawDisplayName, ModTomlFileContent.displayURL, fileName, searchableName);
                }
                else
                {
                    LogFile.LogMessage($"Error, no mod displayName was found in mods.toml for file {fileName}");
                }
            }

            return ModData;
        }
        public string RemoveUndesiredApisFromString(string str)
        {
            string desiredApi = PersistentVariables.GetSelectedApi().ToLower();
            List<string> desiredApiList = new List<string>() { desiredApi };

            foreach (string nonDesiredApi in nonDesiredApis)
            {
                str = Utilities.FastReplace(str, nonDesiredApi, " ");
            }

            return Utilities.RemoveDoubleSpacings(str);
        }

        public ModData GetCurrentlyProcessedModData()
        {
            ModData ModData = null;
            int dataIndex = PersistentVariables.GetCurrentModListIndex();

            if (dataIndex < sourceFolderModData.Count)
            {
                ModData = sourceFolderModData[dataIndex];
            }

            return ModData;
        }
        public string GetFormattedDisplayNameForSearch(string displayName)
        {
            searchTermBlacklist.ForEach(blacklistedStr =>
            {
                displayName = Utilities.FastReplace(displayName, blacklistedStr, " ");
            });

            displayName = Utilities.ToHumanReadable(displayName);
            displayName = Utilities.RemoveVersioning(displayName);
            displayName = Utilities.RemoveIrrelevantCharacters(displayName);
            displayName = Utilities.RemoveDoubleSpacings(displayName);

            return displayName;
        }
        public HashSet<string> GetOutputDependencies(string json)
        {
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(json)!;
            HashSet<string> dependencyModIds = new HashSet<string>();

            if (dynamicObject.dependencies != null)
            {
                foreach (var dependency in dynamicObject.dependencies)
                {
                    foreach (var listing in dependency.Children()[0])
                    {
                        LogFile.LogMessage(JsonConvert.SerializeObject(listing));
                        string modId = (string)listing.modId;
                        if (modId != "minecraft" && modId != "forge")
                        {
                            dependencyModIds.Add(modId);
                        }
                    }
                }
            }

            return dependencyModIds;
        }
        public HashSet<string> GetOutputMissingDependencies()
        {
            string modFolderPath = PersistentVariables.GetOutputModPath();

            List<ModData> ModTomlFileContentDatum = new List<ModData>();
            string[] jarFiles = Directory.GetFiles(modFolderPath, "*.jar")
                         .Select(Path.GetFileName)
                         .ToArray();
            HashSet<string> dependencyModIds = new HashSet<string>();

            foreach (string fileName in jarFiles)
            {
                HashSet<string> outputDependencies = GetOutputDependencies(ReadJarModTomlFileContent(Path.Combine(modFolderPath, fileName)));
                dependencyModIds.Concat(outputDependencies);
            }

            return dependencyModIds;
        }
        public List<string> GetNonDesiredApis()
        {
            return nonDesiredApis;
        }

        private void SkipExisting(Action onSkipExisting)
        {
            int sourceModCount = sourceFolderModData.Count;

            if (PersistentVariables.GetSkipExisting())
            {
                List<ModData> outputFolderModData = GetAllModData(outputModFolderPath, null);
                sourceFolderModData = sourceFolderModData.Where(sourceMod =>
                    (!outputFolderModData.Select(outputMod => outputMod.searchableName).ToList()
                        .Contains(sourceMod.searchableName))).ToList<ModData>();

                int modCountToInstall = sourceFolderModData.Count;

                if (sourceModCount != modCountToInstall)
                {
                    onSkipExisting.Invoke();
                }
            }
        }

        private void InitializeData(Dictionary<string, string> gameFlavorIds)
        {
            string desiredApi = PersistentVariables.GetSelectedApi().ToLower();

            foreach (string key in gameFlavorIds.Keys)
            {
                string api = key.ToLower();

                if (api != desiredApi)
                {
                    nonDesiredApis.Add(api);
                }
            }
        }

        private static string ReadJarModTomlFileContent(string path)
        {
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                ZipArchiveEntry entry = archive.GetEntry("META-INF/mods.toml");
                if (entry != null)
                {
                    using (StreamReader reader = new StreamReader(entry.Open()))
                    {
                        var toml = Tomlyn.Toml.ToModel(reader.ReadToEnd());

                        return JsonConvert.SerializeObject(toml);
                    }
                }
            }

            return "";
        }

    }
}
