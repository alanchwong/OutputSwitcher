namespace OutputSwitcher.TrayApp
{
    partial class UseAppliedPresetCountdownForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UseAppliedPresetCountdownForm));
            this.LabelKeepDisplayConfiguration = new System.Windows.Forms.Label();
            this.LabelCountdownText = new System.Windows.Forms.Label();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonRevert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LabelKeepDisplayConfiguration
            // 
            this.LabelKeepDisplayConfiguration.AutoSize = true;
            this.LabelKeepDisplayConfiguration.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelKeepDisplayConfiguration.Location = new System.Drawing.Point(12, 18);
            this.LabelKeepDisplayConfiguration.Name = "LabelKeepDisplayConfiguration";
            this.LabelKeepDisplayConfiguration.Size = new System.Drawing.Size(160, 13);
            this.LabelKeepDisplayConfiguration.TabIndex = 0;
            this.LabelKeepDisplayConfiguration.Text = "Keep new display configuration?";
            // 
            // LabelCountdownText
            // 
            this.LabelCountdownText.AutoSize = true;
            this.LabelCountdownText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelCountdownText.Location = new System.Drawing.Point(12, 44);
            this.LabelCountdownText.Name = "LabelCountdownText";
            this.LabelCountdownText.Size = new System.Drawing.Size(224, 13);
            this.LabelCountdownText.TabIndex = 1;
            this.LabelCountdownText.Text = "Display configuration will revert in 15 seconds.";
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOK.Location = new System.Drawing.Point(87, 79);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 2;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // ButtonRevert
            // 
            this.ButtonRevert.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonRevert.Location = new System.Drawing.Point(168, 79);
            this.ButtonRevert.Name = "ButtonRevert";
            this.ButtonRevert.Size = new System.Drawing.Size(75, 23);
            this.ButtonRevert.TabIndex = 3;
            this.ButtonRevert.Text = "Revert";
            this.ButtonRevert.UseVisualStyleBackColor = true;
            this.ButtonRevert.Click += new System.EventHandler(this.ButtonRevert_Click);
            // 
            // UseAppliedPresetCountdownForm
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonRevert;
            this.ClientSize = new System.Drawing.Size(255, 118);
            this.Controls.Add(this.ButtonRevert);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.LabelCountdownText);
            this.Controls.Add(this.LabelKeepDisplayConfiguration);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "UseAppliedPresetCountdownForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OutputSwitcher - Keep New Display Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelKeepDisplayConfiguration;
        private System.Windows.Forms.Label LabelCountdownText;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonRevert;
    }
}