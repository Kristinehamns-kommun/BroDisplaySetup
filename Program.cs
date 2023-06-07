using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BroDisplaySetup
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string programName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            Form appForm = Displays.ArrangeManuallyFromLTRWithAutoResolutionForm();

            appForm.Paint += (s, e) =>
            {
                Image image = Properties.Resources.Kristinehamn_CMYK;

                double aspectRatio = (double)image.Width / (double)image.Height;
                int imgWidth = 300;
                int imgHeight = (int)(imgWidth / aspectRatio);

                // Draw the image on the form's surface
                int padding = 40;
                e.Graphics.DrawImage(image, appForm.Width-imgWidth-padding, appForm.Height-imgHeight-padding, imgWidth, imgHeight);
            };

            // Create a MenuStrip control for the menu bar
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Dock = DockStyle.Top;

            ToolStripMenuItem advancedMenuItem = new ToolStripMenuItem("Avancerat");
            menuStrip.Items.Add(advancedMenuItem);

            ToolStripMenuItem showScreenInfoMenuItem = new ToolStripMenuItem("Visa skärminformation...");
            advancedMenuItem.DropDownItems.Add(showScreenInfoMenuItem);

            ToolStripMenuItem helpMenuItem = new ToolStripMenuItem("?");

            ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem($"About {programName}");
            helpMenuItem.DropDownItems.Add(aboutMenuItem);

            menuStrip.Items.Add(helpMenuItem);
            
            showScreenInfoMenuItem.Click += (s, e) =>
            {
                // Create and show the new form for screen info
                ScreenInfoForm screenInfoForm = new ScreenInfoForm();
                screenInfoForm.ShowDialog();
            };

            aboutMenuItem.Click += (s, e) =>
            {
                // Create and show the new form for about
                About aboutForm = new About();
                aboutForm.ShowDialog();
            };

            appForm.Controls.Add(menuStrip);

            int closeButtonSize = 40;
            Button closeButton = new Button();
            closeButton.Size = new Size(closeButtonSize, closeButtonSize);
            closeButton.Location = new Point(appForm.Width - closeButton.Width - (closeButtonSize/2), (closeButtonSize));
            closeButton.Text = ""; // Clear the button's text
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;

            closeButton.Paint += (s, e) =>
            {
                Button button = (Button)s;
                Graphics g = e.Graphics;
                Rectangle rect = button.ClientRectangle;

                // Set the color and thickness of the 'X' shape
                Color xColor = Color.Black;
                int xThickness = 10;
                int padding = 5;

                // Draw the 'X' shape
                using (Pen pen = new Pen(xColor, xThickness))
                {
                    g.DrawLine(pen, rect.Left+padding, rect.Top+padding, rect.Right-padding, rect.Bottom-padding);
                    g.DrawLine(pen, rect.Left+padding, rect.Bottom-padding, rect.Right-padding, rect.Top+padding);
                }
            };

            closeButton.Click += (s, e) =>
            {
                appForm.Close();
            };

            appForm.Controls.Add(closeButton);

            Application.Run(appForm);
        }
    }
}
