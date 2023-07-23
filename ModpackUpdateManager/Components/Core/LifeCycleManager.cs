using ModpackUpdateManager.Enums;
using ModpackUpdateManager.Models;
using ModpackUpdateManager.Utils;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Components
{
    public partial class Core
    {
        private class LifeCycleManager
        {
            private ModOperationManager modOperationManager;
            private Automation automation;
            private UserMessaging userMessaging;
            private ModDataAccessor modDataAccessor;
            private Miscellaneous miscellaneous;

            public LifeCycleManager(ModOperationManager _modOperationManager, Automation _automation,
                UserMessaging _userMessaging, ModDataAccessor _modDataAccessor, Miscellaneous _miscellaneous)
            {
                modOperationManager = _modOperationManager;
                automation = _automation;
                userMessaging = _userMessaging;
                modDataAccessor = _modDataAccessor;
                miscellaneous = _miscellaneous;
            }

            public async Task ManageAutoModeLifecycle()
            {
                TaskResult result = TaskResult.Failure;

                do
                {
                    if (!PersistentVariables.GetIsInAutoMode() || PersistentVariables.GetIsTaskCancelled() || PersistentVariables.GetIsTaskCompleted())
                        continue;

                    await miscellaneous.LoadSearchUrl();

                    ModSearchResult modSearchResult = await automation.AutoSelectMod();

                    if (modSearchResult == null)
                    {
                        modOperationManager.SkipMod(ModCompletionStatus.Failure, "Search yielded no valid results, could not automatically download.");
                        continue;
                    }

                    userMessaging.ShowMessage($"{modSearchResult.Value} automatically selected");

                    if (!PersistentVariables.GetIsInAutoMode() || PersistentVariables.GetIsTaskCancelled())
                        return;

                    result = await modOperationManager.DownloadAndValidate();

                    if (result == TaskResult.Cancelled)
                    {
                        return;
                    }
                    else if (result == TaskResult.Completed)
                    {
                        PersistentVariables.SetIsTaskCompleted(true);
                        continue;
                    }
                    else if (result == TaskResult.Failure)
                    {
                        continue;
                    }

                    if (PersistentVariables.GetCurrentModListIndex() >= modDataAccessor.GetSourceModQuantity())
                    {
                        break;
                    }

                } while ((PersistentVariables.GetIsInAutoMode() && !PersistentVariables.GetIsTaskCancelled()) && !PersistentVariables.GetIsTaskCompleted());

                if (PersistentVariables.GetIsTaskCompleted())
                {
                    userMessaging.ShowMessage("Automatic modpack update successfully completed.");
                    modOperationManager.FinalizeUpdateProcess();
                }

                userMessaging.ShowMessage("Stopping automatic mode...");

            }

            public async Task ManageManualModeLifecycle()
            {
                if (miscellaneous.IsOnModMainPage())
                {
                    TaskResult result = await modOperationManager.DownloadAndValidate();

                    if (result == TaskResult.Completed)
                    {
                        userMessaging.ShowMessage("Manual modpack update successfully completed.");
                        PersistentVariables.SetIsTaskCompleted(true);
                        modOperationManager.FinalizeUpdateProcess();
                    }
                }


                if (!PersistentVariables.GetIsTaskCompleted())
                {
                    await miscellaneous.LoadSearchUrl();
                }
            }

        }
    }
}
