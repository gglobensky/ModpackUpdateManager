namespace ModpackUpdateManager
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbl_Api = new System.Windows.Forms.Label();
            this.lbl_Version = new System.Windows.Forms.Label();
            this.btn_Accept = new System.Windows.Forms.Button();
            this.combo_Api = new System.Windows.Forms.ComboBox();
            this.combo_Version = new System.Windows.Forms.ComboBox();
            this.checkbox_SkipExisting = new System.Windows.Forms.CheckBox();
            this.lbl_OutputPath = new System.Windows.Forms.Label();
            this.lbl_SourcePath = new System.Windows.Forms.Label();
            this.btn_BrowseOutput = new System.Windows.Forms.Button();
            this.txt_OutputPath = new System.Windows.Forms.TextBox();
            this.btn_BrowseSource = new System.Windows.Forms.Button();
            this.txt_SourcePath = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.tip_MainTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel2.Controls.Add(this.lbl_Api);
            this.panel2.Controls.Add(this.lbl_Version);
            this.panel2.Controls.Add(this.btn_Accept);
            this.panel2.Controls.Add(this.combo_Api);
            this.panel2.Controls.Add(this.combo_Version);
            this.panel2.Controls.Add(this.checkbox_SkipExisting);
            this.panel2.Controls.Add(this.lbl_OutputPath);
            this.panel2.Controls.Add(this.lbl_SourcePath);
            this.panel2.Controls.Add(this.btn_BrowseOutput);
            this.panel2.Controls.Add(this.txt_OutputPath);
            this.panel2.Controls.Add(this.btn_BrowseSource);
            this.panel2.Controls.Add(this.txt_SourcePath);
            this.panel2.Location = new System.Drawing.Point(6, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(527, 186);
            this.panel2.TabIndex = 4;
            // 
            // lbl_Api
            // 
            this.lbl_Api.AutoSize = true;
            this.lbl_Api.Location = new System.Drawing.Point(425, 69);
            this.lbl_Api.Name = "lbl_Api";
            this.lbl_Api.Size = new System.Drawing.Size(40, 13);
            this.lbl_Api.TabIndex = 12;
            this.lbl_Api.Text = "For Api";
            // 
            // lbl_Version
            // 
            this.lbl_Version.AutoSize = true;
            this.lbl_Version.Location = new System.Drawing.Point(422, 19);
            this.lbl_Version.Name = "lbl_Version";
            this.lbl_Version.Size = new System.Drawing.Size(54, 13);
            this.lbl_Version.TabIndex = 11;
            this.lbl_Version.Text = "Update to";
            // 
            // btn_Accept
            // 
            this.btn_Accept.Location = new System.Drawing.Point(425, 143);
            this.btn_Accept.Name = "btn_Accept";
            this.btn_Accept.Size = new System.Drawing.Size(75, 23);
            this.btn_Accept.TabIndex = 8;
            this.btn_Accept.Text = "Accept";
            this.btn_Accept.UseVisualStyleBackColor = true;
            this.btn_Accept.Click += new System.EventHandler(this.btn_Accept_Click);
            // 
            // combo_Api
            // 
            this.combo_Api.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_Api.FormattingEnabled = true;
            this.combo_Api.Location = new System.Drawing.Point(425, 84);
            this.combo_Api.Name = "combo_Api";
            this.combo_Api.Size = new System.Drawing.Size(79, 21);
            this.combo_Api.TabIndex = 5;
            // 
            // combo_Version
            // 
            this.combo_Version.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_Version.FormattingEnabled = true;
            this.combo_Version.Location = new System.Drawing.Point(425, 35);
            this.combo_Version.Name = "combo_Version";
            this.combo_Version.Size = new System.Drawing.Size(79, 21);
            this.combo_Version.TabIndex = 2;
            // 
            // checkbox_SkipExisting
            // 
            this.checkbox_SkipExisting.AutoSize = true;
            this.checkbox_SkipExisting.Location = new System.Drawing.Point(23, 126);
            this.checkbox_SkipExisting.Name = "checkbox_SkipExisting";
            this.checkbox_SkipExisting.Size = new System.Drawing.Size(85, 17);
            this.checkbox_SkipExisting.TabIndex = 6;
            this.checkbox_SkipExisting.Text = "Skip existing";
            this.tip_MainTooltip.SetToolTip(this.checkbox_SkipExisting, "Skip existing files in output folder");
            this.checkbox_SkipExisting.UseVisualStyleBackColor = true;
            // 
            // lbl_OutputPath
            // 
            this.lbl_OutputPath.AutoSize = true;
            this.lbl_OutputPath.Location = new System.Drawing.Point(101, 69);
            this.lbl_OutputPath.Name = "lbl_OutputPath";
            this.lbl_OutputPath.Size = new System.Drawing.Size(64, 13);
            this.lbl_OutputPath.TabIndex = 10;
            this.lbl_OutputPath.Text = "Output Path";
            this.tip_MainTooltip.SetToolTip(this.lbl_OutputPath, "Leave blank to output in executable folder");
            // 
            // lbl_SourcePath
            // 
            this.lbl_SourcePath.AutoSize = true;
            this.lbl_SourcePath.Location = new System.Drawing.Point(103, 19);
            this.lbl_SourcePath.Name = "lbl_SourcePath";
            this.lbl_SourcePath.Size = new System.Drawing.Size(66, 13);
            this.lbl_SourcePath.TabIndex = 9;
            this.lbl_SourcePath.Text = "Source Path";
            // 
            // btn_BrowseOutput
            // 
            this.btn_BrowseOutput.Location = new System.Drawing.Point(23, 84);
            this.btn_BrowseOutput.Name = "btn_BrowseOutput";
            this.btn_BrowseOutput.Size = new System.Drawing.Size(75, 23);
            this.btn_BrowseOutput.TabIndex = 3;
            this.btn_BrowseOutput.Text = "Browse";
            this.btn_BrowseOutput.UseVisualStyleBackColor = true;
            this.btn_BrowseOutput.Click += new System.EventHandler(this.btn_BrowseOutput_Click);
            // 
            // txt_OutputPath
            // 
            this.txt_OutputPath.Location = new System.Drawing.Point(104, 85);
            this.txt_OutputPath.Name = "txt_OutputPath";
            this.txt_OutputPath.Size = new System.Drawing.Size(315, 20);
            this.txt_OutputPath.TabIndex = 4;
            // 
            // btn_BrowseSource
            // 
            this.btn_BrowseSource.Location = new System.Drawing.Point(23, 33);
            this.btn_BrowseSource.Name = "btn_BrowseSource";
            this.btn_BrowseSource.Size = new System.Drawing.Size(75, 23);
            this.btn_BrowseSource.TabIndex = 0;
            this.btn_BrowseSource.Text = "Browse";
            this.btn_BrowseSource.UseVisualStyleBackColor = true;
            this.btn_BrowseSource.Click += new System.EventHandler(this.btn_BrowseSource_Click);
            // 
            // txt_SourcePath
            // 
            this.txt_SourcePath.Location = new System.Drawing.Point(103, 35);
            this.txt_SourcePath.Name = "txt_SourcePath";
            this.txt_SourcePath.Size = new System.Drawing.Size(315, 20);
            this.txt_SourcePath.TabIndex = 1;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 193);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(555, 232);
            this.MinimumSize = new System.Drawing.Size(555, 232);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Minecraft Modpack Updater";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox checkbox_SkipExisting;
        private System.Windows.Forms.Label lbl_OutputPath;
        private System.Windows.Forms.Label lbl_SourcePath;
        private System.Windows.Forms.Button btn_BrowseOutput;
        private System.Windows.Forms.TextBox txt_OutputPath;
        private System.Windows.Forms.Button btn_BrowseSource;
        private System.Windows.Forms.TextBox txt_SourcePath;
        private System.Windows.Forms.ComboBox combo_Version;
        private System.Windows.Forms.ComboBox combo_Api;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.Button btn_Accept;
        private System.Windows.Forms.ToolTip tip_MainTooltip;
        private System.Windows.Forms.Label lbl_Api;
        private System.Windows.Forms.Label lbl_Version;
    }
}