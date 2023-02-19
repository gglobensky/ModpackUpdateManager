namespace ModpackUpdateManager
{
    partial class Form1
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
            this.btn_ManualStep = new System.Windows.Forms.Button();
            this.btn_StartAuto = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_StopAuto = new System.Windows.Forms.Button();
            this.txt_URL = new System.Windows.Forms.TextBox();
            this.btn_Forward = new System.Windows.Forms.Button();
            this.btn_Back = new System.Windows.Forms.Button();
            this.btn_SkipMod = new System.Windows.Forms.Button();
            this.txt_output = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_ManualStep
            // 
            this.btn_ManualStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ManualStep.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.btn_ManualStep.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_ManualStep.Location = new System.Drawing.Point(652, 28);
            this.btn_ManualStep.Name = "btn_ManualStep";
            this.btn_ManualStep.Size = new System.Drawing.Size(97, 51);
            this.btn_ManualStep.TabIndex = 0;
            this.btn_ManualStep.Text = "Manual Step";
            this.btn_ManualStep.UseVisualStyleBackColor = false;
            this.btn_ManualStep.Click += new System.EventHandler(this.btn_ManualStep_Click);
            // 
            // btn_StartAuto
            // 
            this.btn_StartAuto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_StartAuto.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.btn_StartAuto.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_StartAuto.Location = new System.Drawing.Point(16, 28);
            this.btn_StartAuto.Name = "btn_StartAuto";
            this.btn_StartAuto.Size = new System.Drawing.Size(97, 51);
            this.btn_StartAuto.TabIndex = 1;
            this.btn_StartAuto.Text = "Start Auto";
            this.btn_StartAuto.UseVisualStyleBackColor = false;
            this.btn_StartAuto.Click += new System.EventHandler(this.btn_StartAuto_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel1.Controls.Add(this.btn_StopAuto);
            this.panel1.Controls.Add(this.txt_URL);
            this.panel1.Controls.Add(this.btn_Forward);
            this.panel1.Controls.Add(this.btn_Back);
            this.panel1.Controls.Add(this.btn_SkipMod);
            this.panel1.Controls.Add(this.txt_output);
            this.panel1.Controls.Add(this.btn_ManualStep);
            this.panel1.Controls.Add(this.btn_StartAuto);
            this.panel1.Location = new System.Drawing.Point(137, 345);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(765, 144);
            this.panel1.TabIndex = 2;
            // 
            // btn_StopAuto
            // 
            this.btn_StopAuto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_StopAuto.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.btn_StopAuto.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_StopAuto.Location = new System.Drawing.Point(53, 101);
            this.btn_StopAuto.Name = "btn_StopAuto";
            this.btn_StopAuto.Size = new System.Drawing.Size(60, 36);
            this.btn_StopAuto.TabIndex = 7;
            this.btn_StopAuto.Text = "Stop";
            this.btn_StopAuto.UseVisualStyleBackColor = false;
            this.btn_StopAuto.Click += new System.EventHandler(this.btn_StopAuto_Click);
            // 
            // txt_URL
            // 
            this.txt_URL.Location = new System.Drawing.Point(119, 4);
            this.txt_URL.Name = "txt_URL";
            this.txt_URL.Size = new System.Drawing.Size(527, 20);
            this.txt_URL.TabIndex = 6;
            this.txt_URL.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_URL_KeyDown);
            // 
            // btn_Forward
            // 
            this.btn_Forward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Forward.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.btn_Forward.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Forward.Location = new System.Drawing.Point(652, 4);
            this.btn_Forward.Name = "btn_Forward";
            this.btn_Forward.Size = new System.Drawing.Size(29, 24);
            this.btn_Forward.TabIndex = 5;
            this.btn_Forward.Text = "=>";
            this.btn_Forward.UseVisualStyleBackColor = false;
            this.btn_Forward.Click += new System.EventHandler(this.btn_Forward_Click);
            // 
            // btn_Back
            // 
            this.btn_Back.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Back.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.btn_Back.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Back.Location = new System.Drawing.Point(84, 3);
            this.btn_Back.Name = "btn_Back";
            this.btn_Back.Size = new System.Drawing.Size(29, 24);
            this.btn_Back.TabIndex = 4;
            this.btn_Back.Text = "<=";
            this.btn_Back.UseVisualStyleBackColor = false;
            this.btn_Back.Click += new System.EventHandler(this.btn_Back_Click);
            // 
            // btn_SkipMod
            // 
            this.btn_SkipMod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_SkipMod.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.btn_SkipMod.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_SkipMod.Location = new System.Drawing.Point(652, 101);
            this.btn_SkipMod.Name = "btn_SkipMod";
            this.btn_SkipMod.Size = new System.Drawing.Size(60, 36);
            this.btn_SkipMod.TabIndex = 3;
            this.btn_SkipMod.Text = "Skip Mod";
            this.btn_SkipMod.UseVisualStyleBackColor = false;
            this.btn_SkipMod.Click += new System.EventHandler(this.btn_SkipMod_Click);
            // 
            // txt_output
            // 
            this.txt_output.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txt_output.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txt_output.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_output.Location = new System.Drawing.Point(118, 28);
            this.txt_output.Multiline = true;
            this.txt_output.Name = "txt_output";
            this.txt_output.ReadOnly = true;
            this.txt_output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_output.Size = new System.Drawing.Size(528, 109);
            this.txt_output.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 501);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_ManualStep;
        private System.Windows.Forms.Button btn_StartAuto;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txt_output;
        private System.Windows.Forms.Button btn_SkipMod;
        private System.Windows.Forms.Button btn_Forward;
        private System.Windows.Forms.Button btn_Back;
        private System.Windows.Forms.Button btn_StopAuto;
        private System.Windows.Forms.TextBox txt_URL;
    }
}

