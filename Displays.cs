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
            List<Dictionary<string, string>> monitorInfoList = MonitorInfoRetriever.GetMonitorInfoForAllConnectedDisplayDevices();
            List<string> autoArrangedScreenDeviceNames = new List<string>();

            foreach (var monitorInfo in monitorInfoList)
            {
                if (monitorInfo.ContainsKey("Internal") && monitorInfo["Internal"].Equals("Yes"))
                {
                    autoArrangedScreenDeviceNames.Add(monitorInfo["DeviceName"]);
                    Screen removePrimaryScreen = screenList.Find(s => s.DeviceName.ToUpper().Equals(monitorInfo["DeviceName"].ToUpper()));

                    //assert that removePrimaryScreen is not null
                    Debug.Assert(removePrimaryScreen != null, "Did not find primary screen");

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
            Debug.Assert(autoArrangedScreenDeviceNames.Count == Screen.AllScreens.Count(), "Missing screen device names");

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

            List<Form> screenForms = new List<Form>();
            
            Color khBroColor = Color.FromArgb(205, 236, 251);
            Color unselectedColor = SystemColors.ControlDark;
            Color selectedColor = khBroColor; // Form.DefaultBackColor;

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
                form.BackColor = unselectedColor;
                form.Show();
                form.Paint += (s, e) =>
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    // Determine the number to display based on the device name
                    int displayIndex = screen.DeviceName.LastIndexOf("DISPLAY");
                    int displayNum = Int32.Parse(screen.DeviceName.Substring(displayIndex + "DISPLAY".Length));

                    // Create the font and brush for drawing
                    //using (Font font = new Font("Gill Sans MT", 256))
                    using (Font font = new Font(SystemFonts.CaptionFont.FontFamily, screenBounds.Height/6, FontStyle.Regular))
                    using (Brush brush = new SolidBrush(Color.Black))
                    {
                        // Get the size of the string when drawn with the given font
                        SizeF stringSize = e.Graphics.MeasureString(currentScreenId.ToString(), font);

                        // Calculate the top-left point of the string to draw it centered in the form
                        float x = (form.Width - stringSize.Width) / 2;
                        float y = ((form.Height - stringSize.Height) / 2) - (stringSize.Height / 8);

                        // Draw the string
                        e.Graphics.DrawString(currentScreenId.ToString(), font, brush, new PointF(x, y));
                    }
                };

                screenForms.Add(form);

                if (screen.Primary)
                {
                    primaryForm = form;
                }

                screenIdForms.Add(form);
            }

            Func<int, Form> selectScreenIdFormByScreenIndex = (screenIdx) => {
                Form screenIdForm = screenIdForms[screenIdx-1];
                screenIdForm.BackColor = selectedColor;
                screenIdForm.Invalidate();
                return screenIdForm;
            };

            Func<int, Form> unselectScreenIdFormByScreenIndex = (screenIdx) => {
                Form screenIdForm = screenIdForms[screenIdx - 1];
                screenIdForm.BackColor = unselectedColor;
                screenIdForm.Invalidate();
                return screenIdForm;
            };

            Func<TextBox, Form> selectScreenIdFormByTextboxValue = (input) => {
                if (input.Text.Length > 0)
                {
                    return selectScreenIdFormByScreenIndex(Int32.Parse(input.Text));
                }

                return null;
            };

            Func<TextBox, Form> unselectScreenIdFormByTextboxValue = (input) => {
                if (input.Text.Length > 0)
                {
                    return unselectScreenIdFormByScreenIndex(Int32.Parse(input.Text));
                }

                return null;
            };

            if (primaryForm != null)
            {
                int closeButtonSize = 60;
                Button closeButton = new Button();
                closeButton.Text = "X";
                closeButton.TextAlign = ContentAlignment.MiddleCenter;
                closeButton.Font = new Font("Arial", 16, FontStyle.Bold);
                closeButton.Size = new Size(closeButtonSize, closeButtonSize);
                closeButton.Location = new Point(primaryForm.Width - closeButton.Width - closeButtonSize, closeButtonSize);

                closeButton.Click += (s, e) =>
                {
                    screenIdForms.ForEach(f => {
                        if (f != primaryForm)
                        {
                            f.Dispose();
                        }

                        primaryForm.Dispose();
                    });
                };
                primaryForm.Controls.Add(closeButton);

                // Create a MenuStrip control for the menu bar
                //MenuStrip menuStrip = new MenuStrip();
                //menuStrip.Dock = DockStyle.Top;
                //primaryForm.Controls.Add(menuStrip);

                TextBox firstTextBox = null;

                TextBox previouslyAddedTextBox = null;

                int inputFontSize = primaryForm.Height / 14;

                int tbWidth = (int)Math.Round(inputFontSize*2.4);
                int tbMargin = (int)Math.Round(tbWidth * 0.2);

                int screenCount = Screen.AllScreens.Count();
                int intTbTotalWidth = ((tbWidth + tbMargin) * screenCount) - tbMargin;
                int startLeft = ((primaryForm.Width - (intTbTotalWidth)) / 2);
                //int startLeft = 0;

                int textboxIndex = 0;

                Screen[] allScreens = Screen.AllScreens;
                int maxScreens = allScreens.Count();

                foreach (Screen screen in allScreens)
                {
                    TextBox textBox = new()
                    {
                        //Font = new Font(FontFamily.GenericSansSerif, 64, FontStyle.Regular),
                        Font = new Font(SystemFonts.CaptionFont.FontFamily, inputFontSize, FontStyle.Regular),
                        Width = tbWidth, // adjust as needed
                        Location = new Point(startLeft + (textboxIndex * (tbWidth + tbMargin)), 128), // adjust as needed
                        Multiline = false,
                        MaxLength = 1,
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White,
                        ForeColor = Color.Black,
                        Tag = textboxIndex,
                        TextAlign = HorizontalAlignment.Center,
                        Margin = new Padding(5, 0, 0, 20),
                    };

                    screenIdTextBoxList.Add(textBox);

                    TextBox previousTextBox = previouslyAddedTextBox;

                    textBox.KeyPress += (s, e) =>
                    {
                        // Only allow 0-9 and backspace
                        if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                        {
                            e.Handled = true; // Cancel the character
                        }

                        else if (char.IsDigit(e.KeyChar))
                        {
                            int typedScreenIndex = int.Parse(e.KeyChar.ToString());

                            if (typedScreenIndex < 0 || typedScreenIndex > maxScreens)
                            {
                                e.Handled = true; // Cancel the character
                            }


                            for(int i = 0; i < screenIdTextBoxList.Count; i++)
                            {
                                if (i != (int)((TextBox)s).Tag)
                                {
                                    if (screenIdTextBoxList[i].Text == e.KeyChar.ToString())
                                    {
                                        e.Handled = true; // Cancel the character
                                        return;
                                    }
                                }
                            }

                            selectScreenIdFormByScreenIndex(typedScreenIndex);
                        }
                        else if (char.IsControl(e.KeyChar))
                        {
                            if (e.KeyChar != (char)Keys.Back)
                            {
                                e.Handled = true; // Cancel the character
                                return;
                            }

                            unselectScreenIdFormByTextboxValue(textBox);

                            if (previousTextBox != null)
                            {
                                unselectScreenIdFormByTextboxValue(previousTextBox);
                                previousTextBox.Focus();
                                previousTextBox.Text = "";
                            }
                        }
                    };

                    textBox.GotFocus += (s, e) =>
                    {
                        // TextBox has received focus and therefore
                        // the user has clicked on it or tabbed to it

                        // Empty this and subsequent textboxes
                        for (int i = (int)textBox.Tag; i < screenIdTextBoxList.Count; i++)
                        {
                            unselectScreenIdFormByTextboxValue(screenIdTextBoxList[i]);
                            screenIdTextBoxList[i].Text = "";
                        }

                        // If any previous textbox is empty, then focus on it
                        if (previousTextBox != null && previousTextBox.Text.Trim().Length == 0)
                        {
                            previousTextBox.Focus();
                        }
                    };

                    if (previousTextBox != null)
                    {
                        previousTextBox.KeyUp += (s, e) =>
                        {
                            if (e.KeyCode != Keys.Back)
                            {
                                if (((TextBox)s).Text.Trim().Length > 0) {
                                    textBox.Focus();
                                }
                                
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
                        if (((TextBox)s).Text.Trim().Length == 0)
                        {
                            return;
                        }

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
            List<Dictionary<string, string>> monitorInfoList = MonitorInfoRetriever.GetMonitorInfoForAllConnectedDisplayDevices();
            int primaryDisplayIndex = 0;

            foreach (var deviceName in screenDeviceNamesLTR)
            {
                bool displayInternal = false;

                foreach(var monitorInfo in monitorInfoList)
                {
                    if (monitorInfo["DeviceName"].Equals(deviceName)) {
                        if (monitorInfo.ContainsKey("Internal") && monitorInfo["Internal"].Equals("Yes"))
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
