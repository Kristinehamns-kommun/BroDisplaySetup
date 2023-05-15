
namespace KhBroDisplaySetup
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnManualArrangeDisplays = new System.Windows.Forms.Button();
            btnScreenInfo = new System.Windows.Forms.Button();
            btnAutoArrangeDisplays = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // btnManualArrangeDisplays
            // 
            btnManualArrangeDisplays.Location = new System.Drawing.Point(59, 12);
            btnManualArrangeDisplays.Name = "btnManualArrangeDisplays";
            btnManualArrangeDisplays.Size = new System.Drawing.Size(195, 23);
            btnManualArrangeDisplays.TabIndex = 0;
            btnManualArrangeDisplays.Text = "Konfigurera skärmar manuellt";
            btnManualArrangeDisplays.UseVisualStyleBackColor = true;
            btnManualArrangeDisplays.Click += btnManualArrangeDisplays_Click;
            // 
            // btnScreenInfo
            // 
            btnScreenInfo.Location = new System.Drawing.Point(59, 80);
            btnScreenInfo.Name = "btnScreenInfo";
            btnScreenInfo.Size = new System.Drawing.Size(195, 24);
            btnScreenInfo.TabIndex = 1;
            btnScreenInfo.Text = "Visa skärminformation...";
            btnScreenInfo.UseVisualStyleBackColor = true;
            btnScreenInfo.Click += btnScreenInfo_Click;
            // 
            // btnAutoArrangeDisplays
            // 
            btnAutoArrangeDisplays.Location = new System.Drawing.Point(59, 38);
            btnAutoArrangeDisplays.Name = "btnAutoArrangeDisplays";
            btnAutoArrangeDisplays.Size = new System.Drawing.Size(195, 23);
            btnAutoArrangeDisplays.TabIndex = 2;
            btnAutoArrangeDisplays.Text = "Konfigurera skärmar automatiskt";
            btnAutoArrangeDisplays.UseVisualStyleBackColor = true;
            btnAutoArrangeDisplays.Click += btnAutoArrangeDisplays_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(316, 118);
            Controls.Add(btnAutoArrangeDisplays);
            Controls.Add(btnScreenInfo);
            Controls.Add(btnManualArrangeDisplays);
            Name = "MainForm";
            Text = "KhBroDisplaySetup";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnManualArrangeDisplays;
        private System.Windows.Forms.Button btnScreenInfo;
        private System.Windows.Forms.Button btnAutoArrangeDisplays;
    }
}

