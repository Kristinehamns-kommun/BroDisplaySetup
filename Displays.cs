using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KhBroDisplaySetup
{
    class Displays
    {
        public static List<String> GetAutoArrangedLTRScreenDeviceNames()
        {
            List<Screen> screenList = Screen.AllScreens.ToList();
            List<Dictionary<string, string>> wmiMonitors = MonitorInfoRetriever.GetWmiMonitorsWithDisplayDeviceName();
            List<string> autoArrangedScreenDeviceNames = new List<string>();

            foreach (var wmiMonitor in wmiMonitors)
            {
                if (wmiMonitor.ContainsKey("Internal") && wmiMonitor["Internal"].Equals("Yes"))
                {
                    autoArrangedScreenDeviceNames.Add(wmiMonitor["DeviceName"]);
                    Screen removePrimaryScreen = screenList.Find(s => s.DeviceName.ToUpper().Equals(wmiMonitor["DeviceName"].ToUpper()));

                    //assert that removePrimaryScreen is not null
                    Debug.Assert(removePrimaryScreen != null);

                    screenList.Remove(removePrimaryScreen);
                    break;
                }
            }

            // Add the remaining screens to the arrangedScreenDevices list:
            foreach (var screen in screenList)
            {
                autoArrangedScreenDeviceNames.Add(screen.DeviceName);
            }

            //Assert that count of arrangedScreenDevices is equal to count of Screen.AllScreens
            Debug.Assert(autoArrangedScreenDeviceNames.Count == Screen.AllScreens.Count());

            return autoArrangedScreenDeviceNames;
        }

        public static void ArrangeAutomaticallyFromLTRWithAutoResolution()
        {
            Extern.Displays.SwitchToExtendModeIfClone();
            ArrangeLTRWithAutoPrimary(GetAutoArrangedLTRScreenDeviceNames());
        }

        public static void ArrangeManuallyFromLTRWithAutoResolution()
        {
            Extern.Displays.SwitchToExtendModeIfClone();
            ConfigureDisplayOrderAndArrange();
        }
        public static void ConfigureDisplayOrderAndArrange()
        {
            List<Form> screenIdForms = new List<Form>();
            List<TextBox> screenIdTextBoxList = new List<TextBox>();

            Form primaryForm = null;

            int screenIndex = 0;

            Dictionary<String, Screen> screenDeviceNameToScreenMap = new Dictionary<string, Screen>();
            // Map the screen device names to the screens:
            foreach (Screen screen in Screen.AllScreens)
            {
                screenDeviceNameToScreenMap.Add(screen.DeviceName, screen);
            }

            List<String> initialArrangement = GetAutoArrangedLTRScreenDeviceNames();

            foreach (String screenDeviceName in initialArrangement)
            {
                Screen screen = screenDeviceNameToScreenMap[screenDeviceName];
                screenIndex++;
                int currentScreenId = screenIndex;
                Rectangle screenBounds = screen.Bounds;
                System.Diagnostics.Debug.WriteLine("Bounds of " + screen.DeviceName + ": " + screenBounds.Left + ", " + screenBounds.Top + ", " + screenBounds.Width + ", " + screenBounds.Height);

                Form form = new Form();
                form.FormBorderStyle = FormBorderStyle.None;
                form.StartPosition = FormStartPosition.Manual;
                form.Bounds = screen.Bounds;
                form.TopMost = false;
                form.Show();
                form.Paint += (s, e) =>
                {
                    // Determine the number to display based on the device name
                    int displayIndex = screen.DeviceName.LastIndexOf("DISPLAY");
                    int displayNum = Int32.Parse(screen.DeviceName.Substring(displayIndex + "DISPLAY".Length));

                    // Create the font and brush for drawing
                    using (Font font = new Font("Arial", 128))
                    using (Brush brush = new SolidBrush(Color.Green))
                    {
                        // Get the size of the string when drawn with the given font
                        SizeF stringSize = e.Graphics.MeasureString(currentScreenId.ToString(), font);

                        // Calculate the top-left point of the string to draw it centered in the form
                        float x = (form.Width - stringSize.Width) / 2;
                        float y = ((form.Height - stringSize.Height) / 2) - (stringSize.Height / 4);

                        // Draw the string
                        e.Graphics.DrawString(currentScreenId.ToString(), font, brush, new PointF(x, y));
                    }
                };

                if (screen.Primary)
                {
                    primaryForm = form;
                }

                screenIdForms.Add(form);
            }

            if (primaryForm != null)
            {
                System.Windows.Forms.TextBox firstTextBox = null;

                System.Windows.Forms.TextBox previouslyAddedTextBox = null;

                int tbWidth = 70;
                int tbMargin = 20;
                int screenCount = Screen.AllScreens.Count();
                int intTbTotalWidth = (tbWidth + tbMargin) * screenCount;
                int startLeft = ((primaryForm.Width - (intTbTotalWidth)) / 2);
                //int startLeft = 0;


                int textboxIndex = 0;

                foreach (Screen screen in Screen.AllScreens)
                {

                    System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox
                    {
                        Font = new Font(FontFamily.GenericSansSerif, 48, FontStyle.Regular),
                        Width = tbWidth, // adjust as needed
                        Location = new Point(startLeft + (textboxIndex * (tbWidth + tbMargin)), 50), // adjust as needed
                        Multiline = false,
                        MaxLength = 1,
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White,
                        ForeColor = Color.Black,
                        Tag = screen.DeviceName
                    };

                    screenIdTextBoxList.Add(textBox);

                    System.Windows.Forms.TextBox previousTextBox = previouslyAddedTextBox;


                    textBox.KeyPress += (s, e) =>
                    {
                        // Only allow 0-9 and backspace
                        if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                        {
                            e.Handled = true; // Cancel the character
                        }
                    };

                    if (previousTextBox != null)
                    {
                        textBox.KeyUp += (s, e) =>
                        {
                            if (e.KeyCode == Keys.Back)
                            {
                                previousTextBox.Focus();
                                //System.Diagnostics.Debug.WriteLine("Move back on keyup");
                            }
                        };

                        previousTextBox.KeyUp += (s, e) =>
                        {
                            if (e.KeyCode != Keys.Back)
                            {
                                textBox.Focus();
                                //System.Diagnostics.Debug.WriteLine("Next on keyup");
                            }
                        };
                    }

                    primaryForm.Controls.Add(textBox);

                    previouslyAddedTextBox = textBox;
                    if (firstTextBox == null)
                    {
                        firstTextBox = textBox;
                    }

                    //System.Diagnostics.Debug.WriteLine("Add tb");

                    textboxIndex++;
                }

                TextBox lastTextBox = previouslyAddedTextBox;

                primaryForm.BringToFront();
                primaryForm.Focus();

                if (firstTextBox != null)
                {
                    firstTextBox.Focus();
                }

                if (lastTextBox != null)
                {
                    lastTextBox.KeyUp += (s, e) =>
                    {
                        var screens = Screen.AllScreens;
                        List<String> userOrderedDeviceNames = new List<String>();

                        int screenOrder = 0;
                        foreach (var screenIdTb in screenIdTextBoxList)
                        {
                            int screenId = Int32.Parse(screenIdTb.Text);
                            System.Diagnostics.Debug.WriteLine(initialArrangement[screenId - 1] + " is screen " + screenOrder);
                            userOrderedDeviceNames.Add(initialArrangement[screenId - 1]);
                            screenOrder++;
                        }

                        screenIdForms.ForEach(f => {
                            if (f != primaryForm)
                            {
                                f.Dispose();
                            }

                            primaryForm.Dispose();
                        });

                        ArrangeLTRWithAutoPrimary(userOrderedDeviceNames);
                    };
                }

            }
        }

        public static void ArrangeLTRWithAutoPrimary(List<String> screenDeviceNamesLTR) {
            List<Dictionary<string, string>> wmiMonitors = MonitorInfoRetriever.GetWmiMonitorsWithDisplayDeviceName();
            int primaryDisplayIndex = 0;

            foreach (var deviceName in screenDeviceNamesLTR)
            {
                bool displayInternal = false;

                foreach(var wmiMonitor in wmiMonitors)
                {
                    if (wmiMonitor["DeviceName"].Equals(deviceName)) {
                        if (wmiMonitor.ContainsKey("Internal") && wmiMonitor["Internal"].Equals("Yes"))
                        {
                            displayInternal = true;
                        }
                    }
                }


                if (displayInternal)
                {
                    primaryDisplayIndex++;
                }
                else
                {
                    break;
                }
            }

            List<string> leftDisplayNames = new();
            List<string> rightDisplayNames = new();

            for(int i = 0; i < screenDeviceNamesLTR.Count; i++)
            {
                if (i < primaryDisplayIndex)
                {
                    leftDisplayNames.Add(screenDeviceNamesLTR[i]);
                }
                else if (i > primaryDisplayIndex)
                {
                    rightDisplayNames.Add(screenDeviceNamesLTR[i]);
                }
            }

            String primaryDisplayName = screenDeviceNamesLTR[primaryDisplayIndex];

            System.Diagnostics.Debug.WriteLine("Primary: ", screenDeviceNamesLTR[primaryDisplayIndex]);
            
            Extern.Displays.Arrange(screenDeviceNamesLTR[primaryDisplayIndex], leftDisplayNames.ToArray(), rightDisplayNames.ToArray());
        }
    }
}
