﻿using ModpackUpdateManager.Managers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ModpackUpdateManager
{
    public partial class Form1 : Form
    {
        private ModOperationManager modOperationManager;
        public Form1(string getModSearchResultFullPath, Dictionary<string, string> gameVersionIds, Dictionary<string, string> gameFlavorIds, List<string> searchTermBlacklist)
        {
            InitializeComponent();
            modOperationManager = new ModOperationManager(this, getModSearchResultFullPath, gameVersionIds, gameFlavorIds, searchTermBlacklist);
            ModeChangeUserOutput();
        }

        public void AddControl(Control control)
        {
            this.Controls.Add(control);
        }

        public void SetOutputText(string message)
        {
            if (txt_output.InvokeRequired)
            {
                Action safeWrite = delegate { SetOutputText($"{message}"); };
                txt_output.Invoke(safeWrite);
            }
            else if (!txt_output.IsDisposed)
            {
                txt_output.AppendText(message + System.Environment.NewLine);
            }
        }

        public void SetURLText(string url)
        {
            if (txt_URL.InvokeRequired)
            {
                Action safeWrite = delegate { SetURLText($"{url}"); };
                txt_URL.Invoke(safeWrite);
            }
            else if (!txt_URL.IsDisposed)
            {
                txt_URL.Text = url;
            }
        }
        public void ToggleControlsEnabled(bool isEnabled)
        {
            SetOutputText(isEnabled ? "IsEnabledForReal" : "NowItAint");
            btn_Back.Enabled = isEnabled;
            btn_Forward.Enabled = isEnabled;
            btn_ManualStep.Enabled = isEnabled;
            btn_SkipMod.Enabled = isEnabled;
            btn_StartAuto.Enabled = isEnabled;
            txt_URL.Enabled = isEnabled;
            btn_StopAuto.Enabled = isEnabled;

            btn_ManualStep.Visible = isEnabled;
        }

        private void ToggleControlsExceptStop(bool isEnabled)
        {
            btn_Back.Enabled = isEnabled;
            btn_Forward.Enabled = isEnabled;
            btn_ManualStep.Enabled = isEnabled;
            btn_SkipMod.Enabled = isEnabled;
            btn_StartAuto.Enabled = isEnabled;
            txt_URL.Enabled = isEnabled;
        }

        private void btn_StartAuto_Click(object sender, EventArgs e)
        {
            modOperationManager.StartAutoMode();
            ModeChangeUserOutput();
        }

        private void btn_ManualStep_Click(object sender, EventArgs e)
        {
            modOperationManager.ManualStep();
            ModeChangeUserOutput();
        }

        private void btn_SkipMod_Click(object sender, EventArgs e)
        {
            modOperationManager.SkipMod("Manual skip");
        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            BrowserManager.Back();
        }

        private void btn_Forward_Click(object sender, EventArgs e)
        {
            BrowserManager.Forward();
        }

        private void btn_StopAuto_Click(object sender, EventArgs e)
        {
            SetOutputText("Stopping automatic mode...");
            modOperationManager.StopAutoMod();
            ModeChangeUserOutput();
        }

        private void txt_URL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BrowserManager.LoadUrl(txt_URL.Text);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ModeChangeUserOutput()
        {
            SetOutputText($"Current operation mode: {(PersistentVariables.GetIsInAutoMode() ? "automatic" : "manual")}");

            if (!PersistentVariables.GetIsInAutoMode())
            {
                SetOutputText("Please press Manual Step or Start Auto to perform operations...");
                ToggleControlsExceptStop(true);
            }
            else
            {
                SetOutputText("Please wait while the system performs the operations...");
                SetOutputText("You can stop at anytime using the Stop button");
                ToggleControlsExceptStop(false);
            }
        }

    }
}