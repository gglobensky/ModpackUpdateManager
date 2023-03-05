using ModpackUpdateManager.Enums;
using ModpackUpdateManager.Models;
using ModpackUpdateManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                await miscellaneous.LoadSearchUrl();

                do
                {
                    if (!PersistentVariables.GetIsInAutoMode() || PersistentVariables.GetIsTaskCancelled())
                        return;

                    ModSearchResult modSearchResult = await automation.TrySelectTargetModFromSearchList();

                    if (modSearchResult == null)
                    {
                        await modOperationManager.SkipMod(ModCompletionStatus.Failure, "Search yielded no valid results, could not automatically download.");
                        continue;
                    }

                    userMessaging.ShowMessage($"{modSearchResult.Value} automatically selected");

                    if (!PersistentVariables.GetIsInAutoMode() || PersistentVariables.GetIsTaskCancelled())
                        return;

                    TaskResult result = await modOperationManager.TryProcessModForTargetedVersion();

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

                    await miscellaneous.LoadSearchUrl();

                } while (PersistentVariables.GetIsInAutoMode() && !PersistentVariables.GetIsTaskCancelled());

                PersistentVariables.SetIsInAutoMode(false);
            }

            public async Task ManageManualModeLifecycle()
            {
                if (miscellaneous.IsOnModMainPage())
                {
                    await modOperationManager.TryProcessModForTargetedVersion();
                }

                await miscellaneous.LoadSearchUrl();
            }
        }
    }
}
