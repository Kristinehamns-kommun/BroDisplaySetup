using System.Drawing;
using System.Drawing.Drawing2D;

namespace BroDisplaySetup
{
    // Shared rounded-rect drawing, factored out of what used to be independently duplicated
    // 4-arc path logic in RoundedTextBox, ScalableCheckBox, and Program.cs's help-text panel.
    internal static class RoundedRectangle
    {
        // Exposed so callers that are themselves clipped to a fixed bounding box (eg. a UserControl,
        // which can't paint outside its own ClientRectangle) know how much margin to reserve for the
        // shadow to actually be visible instead of getting clipped away.
        public const int DefaultShadowOffset = 4;

        public static GraphicsPath CreatePath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90); // Top-left corner
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90); // Top-right corner
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90); // Bottom-right corner
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90); // Bottom-left corner
            path.CloseFigure();

            return path;
        }

        // Draws a soft drop shadow (a semi-transparent offset copy of the same shape) behind the
        // filled/bordered rectangle, so white panels lift off a flat background instead of sitting
        // on it with a hard edge. GDI+ alpha-blends the semi-transparent fill directly onto the
        // destination Graphics surface at draw time, so this works on a normal opaque Form/control
        // without needing a layered window.
        public static void FillWithShadow(Graphics g, RectangleF rect, float radius, Brush fill, Pen border = null, int shadowOffset = DefaultShadowOffset, int shadowAlpha = 70)
        {
            using (GraphicsPath shadowPath = CreatePath(new RectangleF(rect.X + shadowOffset, rect.Y + shadowOffset, rect.Width, rect.Height), radius))
            using (Brush shadowBrush = new SolidBrush(Color.FromArgb(shadowAlpha, Color.Black)))
            {
                g.FillPath(shadowBrush, shadowPath);
            }

            using (GraphicsPath path = CreatePath(rect, radius))
            {
                g.FillPath(fill, path);
                if (border != null)
                {
                    g.DrawPath(border, path);
                }
            }
        }
    }
}
