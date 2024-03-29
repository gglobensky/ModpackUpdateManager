﻿using ModpackUpdateManager.Models;
using ModpackUpdateManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Components
{
    public partial class Core
    {
        private class Automation
        {
            private Dictionary<string, string> gameFlavorIds = new Dictionary<string, string>();
            private List<string> nonDesiredApis = new List<string>();
            private string getModSearchResultScript;
            private ModDataAccessor modDataAccessor;

            public delegate ModSearchResult OnSelectionMade(ModSearchResult modSearchResult);

            public Automation(Dictionary<string, string> _gameFlavorIds, List<string> _nonDesiredApis, string getModSearchResultFullPath, ModDataAccessor _modDataAccessor)
            {
                modDataAccessor = _modDataAccessor;
                gameFlavorIds = _gameFlavorIds;
                nonDesiredApis = _nonDesiredApis;
                getModSearchResultScript = Utilities.ReadFile(getModSearchResultFullPath);
            }
            /// <summary>
            /// Gets the ModSearchResult that matches the most with the mod name
            /// </summary>
            /// <returns>ModSearchResult that corresponds to the mod name from mods.toml</returns>
            public async Task<ModSearchResult> AutoSelectMod()
            {
                ModData modData = modDataAccessor.GetCurrentlyProcessedModData();
                if (modData == null)
                {
                    return null;
                }

                string searchableName = modData.searchableName;
                LogFile.LogMessage($"searchableName: {searchableName}");

                ModSearchResult modSearchResult = await GetAutoSelectedMod(searchableName);

                if (modSearchResult == null)
                {
                    return null;
                }

                await BrowserManager.LoadUrl(modSearchResult.Url);

                return modSearchResult;
            }

            private async Task<ModSearchResult> GetAutoSelectedMod(string searchedTerm, Func<ModSearchResult> onNoSelectionMade = null, bool isRecursion = false)
            {
                LogFile.LogMessage($"Automated mod selection started for {searchedTerm}");

                bool isExactMatch = false;

                List<ModSearchResult> modSearchResults = await GetFormattedModSearchResults();

                ModSearchResult selectedMod = null;

                int? minimumWordDifference = null;

                int len = modSearchResults.Count;

                for (int i = len - 1; i >= 0; i--)
                {
                    ModSearchResult modSearchResult = modSearchResults[i];
                    string targetTerm = modSearchResult.Value;

                    if (IsNonDesiredApi(targetTerm))
                    {
                        LogFile.LogMessage($"Deemed {modSearchResult.Value} as a non desired api version, ignoring...");
                        modSearchResults.RemoveAt(i);
                        continue;
                    }

                    selectedMod = GetSelectedModByWordAnalysis(searchedTerm, targetTerm, ref minimumWordDifference, ref selectedMod, modSearchResult, ref isExactMatch);

                    if (isExactMatch)
                    {
                        break;
                    }
                }

                if (selectedMod != null)
                {
                    LogFile.LogMessage($"Selected mod: {selectedMod.Value} for search term: {searchedTerm}");
                    return selectedMod;
                }

                List<string> sourceWordArray = searchedTerm.Split(' ').ToList<string>();

                if (sourceWordArray.Count > 1)
                {
                    sourceWordArray.RemoveAt(Utilities.GetIndexOfShortestString(sourceWordArray));
                    string newSearchTerm = String.Join(" ", sourceWordArray);

                    LogFile.LogMessage($"Did not find {searchedTerm}, trying with {newSearchTerm}");

                    await BrowserManager.LoadUrl(Utilities.BuildCurseForgeSearchUrl(newSearchTerm));

                    return await GetAutoSelectedMod(newSearchTerm, onNoSelectionMade, true);
                }

                return onNoSelectionMade?.Invoke();
            }

            private bool IsNonDesiredApi(string modName)
            {
                string desiredApi = PersistentVariables.GetSelectedApi();

                bool containingDesiredApi = Utilities.IsSourceInTarget(desiredApi, modName, false, true);

                foreach (string nonDesiredApi in nonDesiredApis)
                {
                    if (Utilities.IsSourceInTarget(nonDesiredApi, modName, false, true) && !containingDesiredApi)
                    {
                        return true;
                    }
                }

                return false;
            }

            private ModSearchResult GetSelectedModByWordAnalysis(string source, string target, ref int? minimumWordDifference, ref ModSearchResult selectedMod, ModSearchResult sourceResult, ref bool isExactMatch)
            {
                LogFile.LogMessage($"Comparing source {source} to target {target}...");
                string desiredApi = PersistentVariables.GetSelectedApi();

                // source already has selected api removed from it's name to broaden the search results on the site
                source = RemoveApis(source, true);
                target = RemoveApis(target, true);

                LogFile.LogMessage($"Removed api names from source, result: {source}");
                LogFile.LogMessage($"Remove undesired api names from target, result: {target}");

                // Check if all words are contained but remove desired api, if foo (forge) is compared against foo, it has to match
                if (Utilities.ContainsAllWords(source, target))
                {
                    LogFile.LogMessage($"Found all words...");
                    int currentWordDifference = Math.Abs(target.Split(' ').Length - source.Split(' ').Length);

                    if (Utilities.IsSourceInTarget(desiredApi, target, false, true) && currentWordDifference == 1)
                    {
                        LogFile.LogMessage($"Target {target} deemed exact match. Selecting...");
                        selectedMod = sourceResult;
                        isExactMatch = true;
                    }
                    else
                    {
                        if (minimumWordDifference == null || currentWordDifference < minimumWordDifference)
                        {
                            minimumWordDifference = currentWordDifference;
                            LogFile.LogMessage($"Target {target} deemed unperfect match, currently selected. Pursuing analysis...");
                            selectedMod = sourceResult;
                        }
                        else if (currentWordDifference == minimumWordDifference)
                        {
                            DateTime currentUpdated = DateTime.Parse(sourceResult.Updated);
                            DateTime selectedUpdated = DateTime.Parse(selectedMod.Updated);

                            if (currentUpdated > selectedUpdated)
                            {
                                selectedMod = sourceResult;
                            }
                        }
                    }
                }
                else
                {
                    LogFile.LogMessage($"Words from \"{source}\" not all found in \"{target}\", returning no selection.");
                }

                return selectedMod;
            }

            private string RemoveApis(string input, bool keepDesired)
            {
                nonDesiredApis.ForEach(api =>
                {
                    input = Utilities.FastReplace(input, api, " ", false, true);
                });

                if (!keepDesired)
                {
                    input = Utilities.FastReplace(input, PersistentVariables.GetSelectedApi(), " ", false, true);
                }

                input = Utilities.RemoveDoubleSpacings(input);

                return input;
            }

            private async Task<List<ModSearchResult>> GetFormattedModSearchResults()
            {
                List<ModSearchResult> modSearchResults = await BrowserManager.ExecuteJavascript<List<ModSearchResult>>(getModSearchResultScript);

                modSearchResults.ForEach(result =>
                {
                    result.Value = modDataAccessor.GetFormattedDisplayNameForSearch(result.Value);
                });

                return modSearchResults;
            }
        }
    }
}