using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.IO;

namespace BroDisplaySetup
{
    public class ScalableCheckBox : UserControl
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                Invalidate(); // Redraw control to reflect state change
            }
        }

        public ScalableCheckBox()
        {
            // Default settings (customize as needed)
            Size = new Size(150, 30);
            Text = "";
            Font = new Font(SystemFonts.CaptionFont.FontFamily, 10, FontStyle.Regular);
            Cursor = Cursors.Hand;
        }

        // Inside your UserControl class or relevant method
        public int GetRequiredWidth()
        {
            // Measure the size of the text
            int textWidth = TextRenderer.MeasureText(this.Text, this.Font).Width;

            int checkBoxTextOffset = 5;

            // Calculate the total width required
            int requiredWidth = getCheckBoxRect().Right + checkBoxTextOffset + textWidth;

            return requiredWidth;
        }

        public int GetRequiredHeight()
        {
            // Measure the size of the text
            int textHeight = TextRenderer.MeasureText(this.Text, this.Font).Height;
            // Calculate the total height required
            int requiredHeight = Math.Max(getCheckBoxRect().Bottom, textHeight);
            return requiredHeight + 5;
        }

        public Size GetRequiredSize()
        {
            return new Size(GetRequiredWidth(), GetRequiredHeight());
        }

        public void SetSizeToRequired()
        {
            Size = GetRequiredSize();
        }

        private Rectangle getCheckBoxRect() {
            // Calculate checkbox size based on font size
            int checkBoxSize = Font.Height;
            Rectangle checkBoxRect = new Rectangle(5, (Height - checkBoxSize) / 2, checkBoxSize, checkBoxSize);
            return checkBoxRect;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(BackColor);
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculate checkbox size based on font size
            int checkBoxSize = Font.Height;
            Rectangle checkBoxRect = getCheckBoxRect();

            // Draw checkbox
            //ControlPaint.DrawCheckBox(g, checkBoxRect, IsChecked ? ButtonState.Checked : ButtonState.Normal);

            // Draw custom checkbox
            using (var path = new GraphicsPath())
            using (Pen pen = new Pen(ForeColor, 2)) // Border color and thickness
            {
                //g.DrawRectangle(pen, checkBoxRect); // Draw the border of the checkbox

                // Calculate the radius based on the size of checkBoxRect, ensuring it's proportionate
                int radius = Math.Min(checkBoxRect.Width, checkBoxRect.Height) / 4; // 1/4th of the smaller dimension

                // Ensure the radius does not exceed half of the smallest dimension to avoid a circle shape
                radius = Math.Min(radius, Math.Min(checkBoxRect.Width, checkBoxRect.Height) / 2);

                // Create a rounded rectangle path fpr the checkbox
                path.AddArc(checkBoxRect.X, checkBoxRect.Y, 2 * radius, 2 * radius, 180, 90); // Top-left corner
                path.AddArc(checkBoxRect.X + checkBoxRect.Width - 2 * radius, checkBoxRect.Y, 2 * radius, 2 * radius, 270, 90); // Top-right corner
                path.AddArc(checkBoxRect.X + checkBoxRect.Width - 2 * radius, checkBoxRect.Y + checkBoxRect.Height - 2 * radius, 2 * radius, 2 * radius, 0, 90); // Bottom-right corner
                path.AddArc(checkBoxRect.X, checkBoxRect.Y + checkBoxRect.Height - 2 * radius, 2 * radius, 2 * radius, 90, 90); // Bottom-left corner
                path.CloseFigure();

                // Fill and draw the checkbox path
                g.FillPath(Brushes.White, path);
                g.DrawPath(pen, path);

                if (IsChecked)
                {
                    // Draw a simple check mark
                    using (Pen checkPen = new Pen(ForeColor, 2)) // Check mark color and thickness
                    {
                        int offset = checkBoxSize / 4;
                        g.DrawLine(checkPen, checkBoxRect.Left + offset, checkBoxRect.Top + offset,
                                           checkBoxRect.Right - offset, checkBoxRect.Bottom - offset);
                        g.DrawLine(checkPen, checkBoxRect.Right - offset, checkBoxRect.Top + offset, checkBoxRect.Left + offset, checkBoxRect.Bottom - offset);
                    }
                }
            }

            // Draw text
            using (Brush textBrush = new SolidBrush(ForeColor))
            {
                g.DrawString(Text, Font, textBrush, checkBoxRect.Right + 5, (Height - Font.Height) / 2);
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            IsChecked = !IsChecked; // Toggle checked state
        }
    }
}
