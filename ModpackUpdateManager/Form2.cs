using ModpackUpdateManager.Components;
using ModpackUpdateManager.Utils;
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
        private const string defaultOutputPath = $".\\output";

        private Dictionary<string, string> gameVersionIds = new Dictionary<string, string>();
        private Dictionary<string, string> gameFlavorIds = new Dictionary<string, string>();

        private Form1 form1;

        private InitContainer initContainer;

        public Form2()
        {
            InitializeComponent();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            initContainer = new InitContainer();
            await initContainer.Initialize(this);

            combo_Version.DataSource = gameVersionIds.Select(element => element.Key).ToList<string>();
            combo_Api.DataSource = gameFlavorIds.Select(element => element.Key).ToList<string>();
        }

        public void SetDataSources(Dictionary<string, string> _gameVersionIds, Dictionary<string, string> _gameFlavorIds)
        {
            gameVersionIds = _gameVersionIds;
            gameFlavorIds = _gameFlavorIds;
        }

        public void SetForm1(Form1 _form1)
        {
            form1 = _form1;
        }

        public void ChooseSourceFolder()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txt_SourcePath.Text = folderBrowserDialog1.SelectedPath;
            }
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

            PersistentVariables.SetDownloadMods(checkbox_DownloadMods.Checked);
            PersistentVariables.SetSkipExisting(checkbox_SkipExisting.Checked);
            PersistentVariables.SetSelectedApi(combo_Api.SelectedValue.ToString());
            PersistentVariables.SetSelectedVersion(combo_Version.SelectedValue.ToString());
            PersistentVariables.SetSourceModPath(txt_SourcePath.Text);
            PersistentVariables.SetOutputModPath(String.IsNullOrEmpty(txt_OutputPath.Text) ?
                defaultOutputPath : txt_OutputPath.Text);


            if (initContainer.CreateMainForm(this))
            {
                this.Hide();
                form1.Closed += (s, args) => this.Close();
                form1.Show();
            }
        }
    }
}
