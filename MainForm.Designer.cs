
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
            this.autoArrangeDisplays = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // autoArrangeDisplays
            // 
            this.autoArrangeDisplays.Location = new System.Drawing.Point(87, 36);
            this.autoArrangeDisplays.Name = "autoArrangeDisplays";
            this.autoArrangeDisplays.Size = new System.Drawing.Size(149, 23);
            this.autoArrangeDisplays.TabIndex = 0;
            this.autoArrangeDisplays.Text = "Konfigurera skärmar";
            this.autoArrangeDisplays.UseVisualStyleBackColor = true;
            this.autoArrangeDisplays.Click += new System.EventHandler(this.autoArrangeDisplays_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 97);
            this.Controls.Add(this.autoArrangeDisplays);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button autoArrangeDisplays;
    }
}

