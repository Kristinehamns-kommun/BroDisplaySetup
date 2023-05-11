
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
            autoArrangeDisplays = new System.Windows.Forms.Button();
            btnScreenInfo = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // autoArrangeDisplays
            // 
            autoArrangeDisplays.Location = new System.Drawing.Point(87, 21);
            autoArrangeDisplays.Name = "autoArrangeDisplays";
            autoArrangeDisplays.Size = new System.Drawing.Size(149, 23);
            autoArrangeDisplays.TabIndex = 0;
            autoArrangeDisplays.Text = "Konfigurera skärmar";
            autoArrangeDisplays.UseVisualStyleBackColor = true;
            autoArrangeDisplays.Click += autoArrangeDisplays_Click;
            // 
            // btnScreenInfo
            // 
            btnScreenInfo.Location = new System.Drawing.Point(87, 50);
            btnScreenInfo.Name = "btnScreenInfo";
            btnScreenInfo.Size = new System.Drawing.Size(149, 24);
            btnScreenInfo.TabIndex = 1;
            btnScreenInfo.Text = "Visa skärminformation...";
            btnScreenInfo.UseVisualStyleBackColor = true;
            btnScreenInfo.Click += btnScreenInfo_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(315, 93);
            Controls.Add(btnScreenInfo);
            Controls.Add(autoArrangeDisplays);
            Name = "MainForm";
            Text = "KhBroDisplaySetup";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button autoArrangeDisplays;
        private System.Windows.Forms.Button btnScreenInfo;
    }
}

