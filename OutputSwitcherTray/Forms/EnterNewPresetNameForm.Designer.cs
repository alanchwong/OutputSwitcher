namespace OutputSwitcher.TrayApp
{
    partial class EnterNewPresetNameForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnterNewPresetNameForm));
            this.TextboxEnterNewPresetName = new System.Windows.Forms.TextBox();
            this.LabelEnterNewPresetName = new System.Windows.Forms.Label();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextboxEnterNewPresetName
            // 
            this.TextboxEnterNewPresetName.Location = new System.Drawing.Point(12, 38);
            this.TextboxEnterNewPresetName.Name = "TextboxEnterNewPresetName";
            this.TextboxEnterNewPresetName.Size = new System.Drawing.Size(454, 20);
            this.TextboxEnterNewPresetName.TabIndex = 0;
            this.TextboxEnterNewPresetName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextboxEnterNewPresetName_KeyDown);
            // 
            // LabelEnterNewPresetName
            // 
            this.LabelEnterNewPresetName.AutoSize = true;
            this.LabelEnterNewPresetName.Location = new System.Drawing.Point(13, 13);
            this.LabelEnterNewPresetName.Name = "LabelEnterNewPresetName";
            this.LabelEnterNewPresetName.Size = new System.Drawing.Size(119, 13);
            this.LabelEnterNewPresetName.TabIndex = 1;
            this.LabelEnterNewPresetName.Text = "Enter new preset name:";
            // 
            // ButtonOK
            // 
            this.ButtonOK.Location = new System.Drawing.Point(310, 72);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 2;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(391, 72);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 3;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // EnterNewPresetNameForm
            // 
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(479, 107);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.LabelEnterNewPresetName);
            this.Controls.Add(this.TextboxEnterNewPresetName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "EnterNewPresetNameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OutputSwitcher - Enter new display preset name";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextboxEnterNewPresetName;
        private System.Windows.Forms.Label LabelEnterNewPresetName;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonCancel;
    }
}