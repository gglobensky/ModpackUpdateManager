using ModpackUpdateManager.Enums;
using ModpackUpdateManager.Models;
using ModpackUpdateManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Components
{
    public partial class Core
    {
        private class Miscellaneous
        {
            private UserMessaging userMessaging;
            private ModDataAccessor modDataAccessor;

            private string reportModPath;
            private Dictionary<ModCompletionStatus, List<LogObject>> report;

            public Miscellaneous(UserMessaging _userMessaging, ModDataAccessor _modDataAccessor, string _reportModPath, Dictionary<ModCompletionStatus, List<LogObject>> _report)
            {
                userMessaging = _userMessaging;
                modDataAccessor = _modDataAccessor;
                reportModPath = _reportModPath;
                report = _report;
            }

            public void ManageReport(ModCompletionStatus status, string modName, string message)
            {
                report[status].Add(new LogObject(modName, message));
                JsonFileHandler.SerializeJsonFile<Dictionary<ModCompletionStatus, List<LogObject>>>(reportModPath, report, false);
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
            public void ShowSearchingUserMessage()
            {
                userMessaging.ShowMessage($"Searching for {modDataAccessor.GetCurrentlyProcessedModData().searchableName}...");
                if (!PersistentVariables.GetIsInAutoMode())
                {
                    userMessaging.ShowMessage($"Please manually select the appropriate mod from the results the click the Step button again.");
                }
            }
            public bool IsDownloadedModDisplayNameMatchingSource(string sourceFileName, string targetFileName)
            {
                string sourcePath = PersistentVariables.GetSourceModPath();
                string targetPath = PersistentVariables.GetOutputModPath();

                ModData sourceModData = modDataAccessor.GetModData(sourcePath, sourceFileName);
                ModData targetModData = modDataAccessor.GetModData(targetPath, targetFileName);

                return sourceModData.displayName == targetModData.displayName;
            }

            public async Task LoadSearchUrl()
            {
                ModData modData = modDataAccessor.GetCurrentlyProcessedModData();
                string searchTerm = modData.searchableName;

                string searchUrl = Utilities.BuildCurseForgeSearchUrl(searchTerm);
                ShowSearchingUserMessage();
                await BrowserManager.LoadUrl(searchUrl);
            }

        }
    }
}
