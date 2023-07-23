using ModpackUpdateManager.Enums;
using ModpackUpdateManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Components
{
    public partial class Core
    {
        public class UserInteraction
        {
            private volatile bool isProcessing = false;
            private volatile bool isStopping = false;

            private ModOperationManager modOperationManager;
            private UserMessaging userMessaging;

            public UserInteraction(string _reportModPath, string getModSearchResultScriptPath, string _invalidDownloadsFolderName, string _dependenciesModPath,
                Dictionary<string, string> gameVersionIds, Dictionary<string, string> gameFlavorIds, List<string> searchTermBlacklist, UserMessaging _userMessaging)
            {
                userMessaging = _userMessaging;

                modOperationManager = new ModOperationManager(_reportModPath, getModSearchResultScriptPath,
                    _invalidDownloadsFolderName, _dependenciesModPath, gameVersionIds, gameFlavorIds, searchTermBlacklist, userMessaging);

            }

            #region Public Methods

            public bool InitializeOperations(Form1 form1)
            {
                return modOperationManager.InitializeOperations(form1);
            }

            public void SkipMod(ModCompletionStatus status, string reason)
            {
                modOperationManager.SkipMod(status, reason);
            }

            public async Task ManualStep()
            {
                if (!isProcessing)
                {
                    isProcessing = true;
                    PersistentVariables.SetIsInAutoMode(false);
                    PersistentVariables.SetIsTaskCancelled(false);

                    await modOperationManager.ManualStep();

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
                    isStopping = false;

                    if (!PersistentVariables.GetIsTaskCompleted())
                    {
                        await modOperationManager.LoadSearchUrl();
                    }
                    
                }
            }

            public async Task StartAutoMode()
            {
                if (!isProcessing)
                {
                    isProcessing = true;

                    PersistentVariables.SetIsInAutoMode(true);
                    PersistentVariables.SetIsTaskCancelled(false);

                    await modOperationManager.StartAutoMode();

                    isProcessing = false;
                }
            }

            public void Dispose()
            {
                BrowserManager.Dispose();
            }

            #endregion
        }
    }
}