using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
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
                Image logoImage = Properties.Resources.Kristinehamn_CMYK;

                double aspectRatio = (double)logoImage.Width / (double)logoImage.Height;
                int logoWidth = (int)Math.Round(appForm.Width*0.15);
                int logoHeight = (int)(logoWidth / aspectRatio);

                // Draw the image on the form's surface
                int padding = Math.Min(40, (int)Math.Round(appForm.Width*0.02));
                e.Graphics.DrawImage(logoImage, appForm.Width-logoWidth-padding, appForm.Height-logoHeight-padding, logoWidth, logoHeight);

                int maxHelpTextWidth = appForm.Width - (2 * padding) - logoWidth;

                string helpText = Properties.Resources.ShortHelp;

                using (Font font = new Font(SystemFonts.DefaultFont.FontFamily, (float)(SystemFonts.DefaultFont.SizeInPoints * 2), FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    // Get the size of the string when drawn with the given font
                    //SizeF stringSize = e.Graphics.MeasureString(helpText, font);
                    float x = padding;
                    //float y = appForm.Height - stringSize.Height - padding;
                    float y = appForm.Height - padding;
                    // Draw the string
                    //e.Graphics.DrawString(helpText, font, brush, new PointF(x, y));
                    drawStringParagraphsWithinMaxWidth(e.Graphics, helpText.Split(Environment.NewLine+Environment.NewLine).ToList(), font, brush, new PointF(x, y), maxHelpTextWidth);
                }
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

        private static void drawStringParagraphsWithinMaxWidth(Graphics graphics, List<string> paragraphs, Font font, Brush brush, PointF lastParagraphLocation, int maxParagraphWidth)
        {
            PointF boxLocation = lastParagraphLocation;

            int textSidePadding=20;
            int textTopPadding=10;
            int textBottomPadding=10;

            Rectangle textPadding = new Rectangle(textSidePadding, textTopPadding, textSidePadding, textBottomPadding);
            

            PointF paragraphLocation = new PointF(lastParagraphLocation.X + textPadding.Left, lastParagraphLocation.Y - textPadding.Top);
            maxParagraphWidth = maxParagraphWidth - textPadding.Left - textPadding.Right;
            SizeF paragraphSize = new SizeF(0, 0);
            float paragraphSpacing = 0.4f;

            // Compute the font size that fits the maximum width
            int fontSize = (int)font.Size;
            bool fitWithinMaxWidth = false;

            while (!fitWithinMaxWidth && fontSize > 8)
            {
                using (Font adjustedFont = new Font(font.FontFamily, fontSize, font.Style))
                {
                    // Check if all lines fit within the maximum width
                    fitWithinMaxWidth = true;

                    foreach (string paragraph in paragraphs)
                    {
                        int paragraphWidth = (int)Math.Round(graphics.MeasureString(paragraph, adjustedFont).Width);

                        if (paragraphWidth > maxParagraphWidth)
                        {
                            fitWithinMaxWidth = false;
                            break;
                        }
                    }
                }

                if (!fitWithinMaxWidth)
                {
                    fontSize--; // Decrease the font size and check again
                }
            }


            // Use adjusted font size
            using (font = new Font(font.FontFamily, fontSize, font.Style)) { 
               
                // Compute size of the text used for drawing the box
                int textTotalHeight = 0;
                int textMaxWidth = 0;

                // The locations of the paragraphs
                PointF[] paragraphLocations = new PointF[paragraphs.Count];

                for (int i = paragraphs.Count - 1; i >= 0; i--)
                {
                    float useParagraphSpacing = paragraphSpacing;
                    if (i == paragraphs.Count - 1)
                    {
                        // No spacing after the last paragraph
                        useParagraphSpacing = 0;
                    }

                    paragraphSize = graphics.MeasureString(paragraphs[i], font);
                    paragraphLocation = new PointF(paragraphLocation.X, paragraphLocation.Y - paragraphSize.Height - (paragraphSize.Height * useParagraphSpacing));
                    textTotalHeight += (int)Math.Round(paragraphSize.Height + (paragraphSize.Height * useParagraphSpacing));

                    paragraphLocations[i] = paragraphLocation;

                    textMaxWidth = Math.Max(textMaxWidth, (int)Math.Round(paragraphSize.Width));
                }

                RectangleF rect = new RectangleF(paragraphLocations[0].X-textPadding.Left, paragraphLocations[0].Y-textPadding.Top, textMaxWidth+textPadding.Right, textTotalHeight+textPadding.Bottom);
                int radius = 10;

                // Draw the rounded rectangle as the background
                using (GraphicsPath path = new GraphicsPath())
                using (Pen pen = new Pen(Color.White))
                {
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90); // Top-left corner
                    path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90); // Top-right corner
                    path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90); // Bottom-right corner
                    path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90); // Bottom-left corner
                    path.CloseFigure();

                    graphics.FillPath(Brushes.White, path);
                    graphics.DrawPath(pen, path);
                }

                for (int i = paragraphs.Count - 1; i >= 0; i--)
                {
                    // Draw the line of text
                    graphics.DrawString(paragraphs[i], font, brush, paragraphLocations[i]);
                }
            }
        }
    }
}
