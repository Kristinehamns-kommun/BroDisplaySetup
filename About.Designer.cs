namespace KhBroDisplaySetup
{
    partial class About
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
            licenseGroupBox = new System.Windows.Forms.GroupBox();
            GplV3TextBox = new System.Windows.Forms.TextBox();
            okButton = new System.Windows.Forms.Button();
            licenseGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // licenseGroupBox
            // 
            licenseGroupBox.Controls.Add(GplV3TextBox);
            licenseGroupBox.Location = new System.Drawing.Point(13, 12);
            licenseGroupBox.Name = "licenseGroupBox";
            licenseGroupBox.Size = new System.Drawing.Size(360, 287);
            licenseGroupBox.TabIndex = 0;
            licenseGroupBox.TabStop = false;
            licenseGroupBox.Text = "GNU General Public License";
            // 
            // GplV3TextBox
            // 
            GplV3TextBox.BackColor = System.Drawing.SystemColors.Control;
            GplV3TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            GplV3TextBox.CausesValidation = false;
            GplV3TextBox.Location = new System.Drawing.Point(6, 22);
            GplV3TextBox.Multiline = true;
            GplV3TextBox.Name = "GplV3TextBox";
            GplV3TextBox.ReadOnly = true;
            GplV3TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            GplV3TextBox.Size = new System.Drawing.Size(348, 259);
            GplV3TextBox.TabIndex = 0;
            // 
            // okButton
            // 
            okButton.Location = new System.Drawing.Point(156, 305);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 30);
            okButton.TabIndex = 1;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // About
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(384, 347);
            Controls.Add(okButton);
            Controls.Add(licenseGroupBox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "About";
            ShowInTaskbar = false;
            Text = "About";
            Load += About_Load;
            licenseGroupBox.ResumeLayout(false);
            licenseGroupBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox licenseGroupBox;
        private System.Windows.Forms.TextBox GplV3TextBox;
        private System.Windows.Forms.Button okButton;
    }
}