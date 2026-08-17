using System;
using System.Drawing;

namespace BroDisplaySetup
{
    // Colors and heading typeface from the Kristinehamns kommun visual identity guide, centralized
    // here so Displays.cs, Program.cs, and the custom controls all draw from the same palette
    // instead of each hard-coding their own copy of the brand blue.
    internal static class BrandStyle
    {
        public static readonly Color PrimaryBlue = Color.FromArgb(129, 207, 244);       // #81CFF4
        public static readonly Color SupplementaryBlue = Color.FromArgb(0, 138, 209);   // #008AD1
        public static readonly Color LightGray = Color.FromArgb(217, 218, 213);         // #D9DAD5
        public static readonly Color PaleBlue = Color.FromArgb(205, 236, 251);          // #CDECFB

        private static readonly Lazy<FontFamily> headingFontFamily = new(() => ResolveFontFamily("Gill Sans MT"));

        public static FontFamily HeadingFontFamily => headingFontFamily.Value;

        // new FontFamily(name) throws if the family isn't installed, unlike new Font(name, ...),
        // which silently substitutes without a reliable way to detect it - the guide asks for an
        // explicit substitute rather than claiming exact brand compliance, so this needs to be
        // detectable.
        private static FontFamily ResolveFontFamily(string name)
        {
            try
            {
                return new FontFamily(name);
            }
            catch (ArgumentException)
            {
                return SystemFonts.CaptionFont.FontFamily;
            }
        }
    }
}
