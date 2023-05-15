namespace KhBroDisplaySetup
{
    partial class ScreenInfoForm
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
            txtScreenInfo = new System.Windows.Forms.TextBox();
            btnOK = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // txtScreenInfo
            // 
            txtScreenInfo.Location = new System.Drawing.Point(11, 11);
            txtScreenInfo.Multiline = true;
            txtScreenInfo.Name = "txtScreenInfo";
            txtScreenInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtScreenInfo.Size = new System.Drawing.Size(647, 311);
            txtScreenInfo.TabIndex = 0;
            // 
            // btnOK
            // 
            btnOK.Location = new System.Drawing.Point(583, 328);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(75, 39);
            btnOK.TabIndex = 1;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // ScreenInfoForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(670, 379);
            Controls.Add(btnOK);
            Controls.Add(txtScreenInfo);
            Name = "ScreenInfoForm";
            Text = "Skärminformation";
            Load += ScreenInfoForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtScreenInfo;
        private System.Windows.Forms.Button btnOK;
    }
}