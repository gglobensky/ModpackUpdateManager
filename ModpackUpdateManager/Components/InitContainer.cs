using ModpackUpdateManager.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModpackUpdateManager.Components.Core;

namespace ModpackUpdateManager.Components
{
    public class InitContainer
    {
        private UserMessaging userMessaging;

        private const string invalidDownloadsFolderName = "invalid";
        private const string logFolderName = "logs";

        private const string executionLogFileName = "execution.log";
        private const string reportLogFileName = "report.log";
        private const string dependenciesLogFileName = "dependencies.log";

        private string reportModPath = Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName, reportLogFileName);
        private string dependenciesModPath = Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName, dependenciesLogFileName);
        private string executionLogFullPath = Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName, executionLogFileName);

        private const string dataFolderPath = ".\\data";
        private const string scriptFolderPath = ".\\scripts";

        private const string getModSearchResultScriptPath = $"{scriptFolderPath}\\GetModSearchResults.js";

        private const string gameVersionIdsJsonPath = $"{dataFolderPath}\\gameVersionIds.json";
        private const string gameFlavorIdsJsonPath = $"{dataFolderPath}\\gameFlavorIds.json";
        private const string searchTermBlacklistJsonPath = $"{dataFolderPath}\\searchTermBlacklist.json";

        private Dictionary<string, string> gameVersionIds = new Dictionary<string, string>()
        {
            {"1.19.2", "9366"},
            {"1.19"  , "9186"},
            {"1.18.2", "9008"},
            {"1.18.1", "8857"},
            {"1.18"  , "8830"},
            {"1.16.5", "8203"},
            {"1.16.4", "8134"},
            {"1.16.3", "8056"},
            {"1.16.1", "7892"},
            {"1.15.2", "7722"},
            {"1.14.4", "7469"},
            {"1.12.2", "6756"},
            {"1.12.1", "6756"},
            {"1.12"  , "6580"}
        };
        private Dictionary<string, string> gameFlavorIds = new Dictionary<string, string>()
        {
            {"Forge", "1" },
            {"Fabric", "4" },
            {"Quilt", "5" }
        };

        private List<string> searchTermBlacklist = new List<string>(){
            "edition",
            "port",
            "unofficial"
        };

        private string getModSearchResultDefaultScript = @"
            (function () {
                let divElements = document.querySelectorAll('div.project-card');

                let result = [];
                let index = 0;

                Array.from(divElements).forEach(element => {
                    let a = element.getElementsByTagName('a')[0];
                    let li = element.querySelector('li.detail-updated');
                    let object = { value: a.innerText, url: a.href, updated: li.getElementsByTagName('span')[0].innerText };
                    result[index++] = object;
                });

                return result;
            })();
            ";

        public async Task Initialize(Form2 form2)
        {
            CreateDataFolder();
            CreateScriptFolder();
            InitializeExternalData();
            await InitializeOutputFiles();
            form2.SetDataSources(gameVersionIds, gameFlavorIds);
        }

        public void CreateMainForm(Form2 form2)
        {
            Form1 form1 = new Form1();

            UserInteraction userInteraction = new UserInteraction(reportModPath, getModSearchResultScriptPath, invalidDownloadsFolderName,
                dependenciesModPath, gameVersionIds, gameFlavorIds, searchTermBlacklist, new UserMessaging(form1));

            form1.SetUserInteraction(userInteraction);
            form2.SetForm1(form1);
            userInteraction.InitializeOperations(form1);
        }

        private async Task InitializeOutputFiles()
        {
            Directory.CreateDirectory(Path.Combine(Path.GetFullPath(PersistentVariables.GetOutputModPath()), logFolderName));
            await JsonFileHandler.CreateFile(reportModPath);
            await JsonFileHandler.CreateFile(dependenciesModPath);
            await JsonFileHandler.CreateFile(executionLogFullPath);
            LogFile.Initialize(executionLogFullPath);
        }

        private void InitializeExternalData()
        {
            if (!File.Exists(gameVersionIdsJsonPath))
            {
                JsonFileHandler.SerializeJsonFile(gameVersionIdsJsonPath, gameVersionIds, false);
            }
            else
            {
                gameVersionIds = JsonFileHandler.DeserializeJsonFile<Dictionary<string, string>>(gameVersionIdsJsonPath);
            }

            if (!File.Exists(gameFlavorIdsJsonPath))
            {
                JsonFileHandler.SerializeJsonFile(gameFlavorIdsJsonPath, gameFlavorIds, false);
            }
            else
            {
                gameFlavorIds = JsonFileHandler.DeserializeJsonFile<Dictionary<string, string>>(gameFlavorIdsJsonPath);
            }

            if (!File.Exists(searchTermBlacklistJsonPath))
            {
                JsonFileHandler.SerializeJsonFile(searchTermBlacklistJsonPath, searchTermBlacklist, false);
            }
            else
            {
                searchTermBlacklist = JsonFileHandler.DeserializeJsonFile<List<string>>(searchTermBlacklistJsonPath);
            }

            if (!File.Exists(getModSearchResultScriptPath))
            {
                Utilities.FileWriteAsync(getModSearchResultScriptPath, getModSearchResultDefaultScript, false).Wait();
            }
        }
        private void CreateDataFolder()
        {
            DirectoryInfo di = Directory.CreateDirectory(dataFolderPath);
        }

        private void CreateScriptFolder()
        {
            DirectoryInfo di = Directory.CreateDirectory(scriptFolderPath);
        }
    }
}
