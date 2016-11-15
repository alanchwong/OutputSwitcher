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
            this.clearHotkeyButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // presetsComboBox
            // 
            this.presetsComboBox.FormattingEnabled = true;
            this.presetsComboBox.Location = new System.Drawing.Point(22, 80);
            this.presetsComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.presetsComboBox.Name = "presetsComboBox";
            this.presetsComboBox.Size = new System.Drawing.Size(302, 28);
            this.presetsComboBox.TabIndex = 0;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(18, 14);
            this.descriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(346, 20);
            this.descriptionLabel.TabIndex = 1;
            this.descriptionLabel.Text = "Select a preset to view and set its global hotkey.";
            // 
            // selectedPresetHotkeyTextbox
            // 
            this.selectedPresetHotkeyTextbox.Location = new System.Drawing.Point(369, 82);
            this.selectedPresetHotkeyTextbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectedPresetHotkeyTextbox.Multiline = true;
            this.selectedPresetHotkeyTextbox.Name = "selectedPresetHotkeyTextbox";
            this.selectedPresetHotkeyTextbox.ReadOnly = true;
            this.selectedPresetHotkeyTextbox.Size = new System.Drawing.Size(606, 29);
            this.selectedPresetHotkeyTextbox.TabIndex = 2;
            // 
            // newHotkeyTextBox
            // 
            this.newHotkeyTextBox.Location = new System.Drawing.Point(369, 155);
            this.newHotkeyTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.newHotkeyTextBox.Name = "newHotkeyTextBox";
            this.newHotkeyTextBox.ReadOnly = true;
            this.newHotkeyTextBox.Size = new System.Drawing.Size(606, 26);
            this.newHotkeyTextBox.TabIndex = 3;
            this.newHotkeyTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newHotkeyTextBox_KeyDown);
            // 
            // setHotkeyButton
            // 
            this.setHotkeyButton.Location = new System.Drawing.Point(864, 195);
            this.setHotkeyButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.setHotkeyButton.Name = "setHotkeyButton";
            this.setHotkeyButton.Size = new System.Drawing.Size(112, 35);
            this.setHotkeyButton.TabIndex = 4;
            this.setHotkeyButton.Text = "Set Hotkey";
            this.setHotkeyButton.UseVisualStyleBackColor = true;
            this.setHotkeyButton.Click += new System.EventHandler(this.setHotkeyButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 55);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Preset:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(364, 55);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Current Global Hotkey";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(364, 131);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "New Global Hotkey";
            // 
            // clearHotkeyButton
            // 
            this.clearHotkeyButton.Location = new System.Drawing.Point(713, 195);
            this.clearHotkeyButton.Name = "clearHotkeyButton";
            this.clearHotkeyButton.Size = new System.Drawing.Size(133, 34);
            this.clearHotkeyButton.TabIndex = 8;
            this.clearHotkeyButton.Text = "Clear Hotkey";
            this.clearHotkeyButton.UseVisualStyleBackColor = true;
            this.clearHotkeyButton.Click += new System.EventHandler(this.clearHotkeyButton_Click);
            // 
            // PresetHotkeyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(999, 249);
            this.Controls.Add(this.clearHotkeyButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.setHotkeyButton);
            this.Controls.Add(this.newHotkeyTextBox);
            this.Controls.Add(this.selectedPresetHotkeyTextbox);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.presetsComboBox);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
        private System.Windows.Forms.Button clearHotkeyButton;
    }
}