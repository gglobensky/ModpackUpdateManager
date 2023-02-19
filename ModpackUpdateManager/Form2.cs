using ModpackUpdateManager.Managers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ModpackUpdateManager
{
    public partial class Form2 : Form
    {
        private const string malformedDirectoryMessage = "{0} is malformed. Please select an existing directory.";
        private const string malformedDirectoriesMessage = "Source and output paths are malformed. Please select existing directories.";

        private const string dataFolderPath = ".\\data";
        private const string scriptFolderPath = ".\\scripts";

        private const string getModSearchResultScriptPath = $"{scriptFolderPath}\\GetModSearchResults.js";

        private const string gameVersionIdsJsonPath = $"{dataFolderPath}\\gameVersionIds.json";
        private const string gameFlavorIdsJsonPath = $"{dataFolderPath}\\gameFlavorIds.json";
        private const string searchTermBlacklistJsonPath = $"{dataFolderPath}\\searchTermBlacklist.json";

        private const string defaultOutputPath = $".\\output";

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

        public Form2()
        {
            InitializeComponent();
            CreateDataFolder();
            CreateScriptFolder();
            InitializeExternalData();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            combo_Version.DataSource = gameVersionIds.Select(element => element.Key).ToList<string>();
            combo_Api.DataSource = gameFlavorIds.Select(element => element.Key).ToList<string>();
        }

        public void ChooseSourceFolder()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txt_SourcePath.Text = folderBrowserDialog1.SelectedPath;
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

        private void btn_BrowseSource_Click(object sender, EventArgs e)
        {
            ChooseSourceFolder();
        }

        public void ChooseOutputFolder()
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                txt_OutputPath.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private void btn_BrowseOutput_Click(object sender, EventArgs e)
        {
            ChooseOutputFolder();
        }

        private void btn_Accept_Click(object sender, EventArgs e)
        {
            bool sourcePathMalformed = !Directory.Exists(txt_SourcePath.Text);
            bool outputPathMalformed = !String.IsNullOrEmpty(txt_OutputPath.Text) && !Directory.Exists(txt_OutputPath.Text);

            if (sourcePathMalformed && outputPathMalformed)
            {
                MessageBox.Show(malformedDirectoriesMessage);
                return;
            }
            else if (sourcePathMalformed)
            {
                MessageBox.Show(String.Format(malformedDirectoryMessage, "Source"));
                return;
            }
            else if (outputPathMalformed)
            {
                MessageBox.Show(String.Format(malformedDirectoryMessage, "Output"));
                return;
            }

            PersistentVariables.SetSkipExisting(checkbox_SkipExisting.Checked);
            PersistentVariables.SetIsVerboseLogging(checkbox_VerboseLog.Checked);
            PersistentVariables.SetSelectedApi(combo_Api.SelectedValue.ToString());
            PersistentVariables.SetSelectedVersion(combo_Version.SelectedValue.ToString());
            PersistentVariables.SetSourceModPath(txt_SourcePath.Text);
            PersistentVariables.SetOutputModPath(String.IsNullOrEmpty(txt_OutputPath.Text) ?
                defaultOutputPath : txt_OutputPath.Text);

            this.Hide();
            Form1 form1 = new Form1(getModSearchResultScriptPath, gameVersionIds, gameFlavorIds, searchTermBlacklist);

            form1.Closed += (s, args) => this.Close();
            form1.Show();
        }
        private void InitializeExternalData()
        {
            if (!File.Exists(gameVersionIdsJsonPath))
            {
                JsonFileHandler.SerializeJsonFile(gameVersionIdsJsonPath, gameVersionIds);
            }
            else
            {
                gameVersionIds = JsonFileHandler.DeserializeJsonFile<Dictionary<string, string>>(gameVersionIdsJsonPath);
            }

            if (!File.Exists(gameFlavorIdsJsonPath))
            {
                JsonFileHandler.SerializeJsonFile(gameFlavorIdsJsonPath, gameFlavorIds);
            }
            else
            {
                gameFlavorIds = JsonFileHandler.DeserializeJsonFile<Dictionary<string, string>>(gameFlavorIdsJsonPath);
            }

            if (!File.Exists(searchTermBlacklistJsonPath))
            {
                JsonFileHandler.SerializeJsonFile(searchTermBlacklistJsonPath, searchTermBlacklist);
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

    }
}
