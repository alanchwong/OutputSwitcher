namespace OutputSwitcher.TrayApp
{
    partial class PresetHotkeyForm
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
            this.presetsComboBox = new System.Windows.Forms.ComboBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.selectedPresetHotkeyTextbox = new System.Windows.Forms.TextBox();
            this.newHotkeyTextBox = new System.Windows.Forms.TextBox();
            this.setHotkeyButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // presetsComboBox
            // 
            this.presetsComboBox.FormattingEnabled = true;
            this.presetsComboBox.Location = new System.Drawing.Point(15, 52);
            this.presetsComboBox.Name = "presetsComboBox";
            this.presetsComboBox.Size = new System.Drawing.Size(203, 21);
            this.presetsComboBox.TabIndex = 0;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(12, 9);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(235, 13);
            this.descriptionLabel.TabIndex = 1;
            this.descriptionLabel.Text = "Select a preset to view and set its global hotkey.";
            // 
            // selectedPresetHotkeyTextbox
            // 
            this.selectedPresetHotkeyTextbox.Location = new System.Drawing.Point(246, 53);
            this.selectedPresetHotkeyTextbox.Multiline = true;
            this.selectedPresetHotkeyTextbox.Name = "selectedPresetHotkeyTextbox";
            this.selectedPresetHotkeyTextbox.ReadOnly = true;
            this.selectedPresetHotkeyTextbox.Size = new System.Drawing.Size(405, 20);
            this.selectedPresetHotkeyTextbox.TabIndex = 2;
            // 
            // newHotkeyTextBox
            // 
            this.newHotkeyTextBox.Location = new System.Drawing.Point(246, 101);
            this.newHotkeyTextBox.Name = "newHotkeyTextBox";
            this.newHotkeyTextBox.ReadOnly = true;
            this.newHotkeyTextBox.Size = new System.Drawing.Size(405, 20);
            this.newHotkeyTextBox.TabIndex = 3;
            this.newHotkeyTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newHotkeyTextBox_KeyDown);
            // 
            // setHotkeyButton
            // 
            this.setHotkeyButton.Location = new System.Drawing.Point(576, 127);
            this.setHotkeyButton.Name = "setHotkeyButton";
            this.setHotkeyButton.Size = new System.Drawing.Size(75, 23);
            this.setHotkeyButton.TabIndex = 4;
            this.setHotkeyButton.Text = "Set Hotkey";
            this.setHotkeyButton.UseVisualStyleBackColor = true;
            this.setHotkeyButton.Click += new System.EventHandler(this.setHotkeyButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Preset:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(243, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Current Global Hotkey";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(243, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "New Global Hotkey";
            // 
            // PresetHotkeyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 162);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.setHotkeyButton);
            this.Controls.Add(this.newHotkeyTextBox);
            this.Controls.Add(this.selectedPresetHotkeyTextbox);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.presetsComboBox);
            this.Name = "PresetHotkeyForm";
            this.Text = "OutputSwitcher - Global Hotkeys";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox presetsComboBox;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.TextBox selectedPresetHotkeyTextbox;
        private System.Windows.Forms.TextBox newHotkeyTextBox;
        private System.Windows.Forms.Button setHotkeyButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}