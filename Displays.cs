﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace BroDisplaySetup
{
    class Displays
    {
        public static List<String> GetAutoArrangedLTRScreenDeviceNames()
        {
            List<Screen> screenList = Screen.AllScreens.ToList();
            List<DisplayInfo> displayInfoList = DisplayInfo.GetDisplayInfoForAllConnectedDisplayDevices();
            List<string> autoArrangedScreenDeviceNames = new List<string>();

            foreach (var displayInfo in displayInfoList)
            {
                if (displayInfo.Internal)
                {
                    autoArrangedScreenDeviceNames.Add(displayInfo.DeviceName);
                    Screen removePrimaryScreen = screenList.Find(s => s.DeviceName.ToUpper().Equals(displayInfo.DeviceName.ToUpper()));

                    //assert that removePrimaryScreen is not null
                    Debug.Assert(removePrimaryScreen != null, "Current primary screen found");

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
            Debug.Assert(autoArrangedScreenDeviceNames.Count == Screen.AllScreens.Count(), "All screen device names processed/found");

            return autoArrangedScreenDeviceNames;
        }

        public static void ArrangeAutomaticallyFromLTRWithAutoResolution()
        {
            Extern.Displays.SwitchToExtendModeIfClone();
            ArrangeLTRWithAutoPrimary(GetAutoArrangedLTRScreenDeviceNames());
        }

        public static Form ArrangeManuallyFromLTRWithAutoResolutionForm()
        {
            Extern.Displays.SwitchToExtendModeIfClone();
            return ConfigureDisplayOrderAndArrangeForm();
        }
        public static Form ConfigureDisplayOrderAndArrangeForm()
        {
            List<Form> screenIdForms = new();
            List<RoundedTextBox> screenIdTextBoxList = new();

            Form primaryForm = null;
            int primarySceenIdTextY = 0;

            int screenIndex = 0;

            Dictionary<String, Screen> screenDeviceNameToScreenMap = new Dictionary<string, Screen>();
            // Map the screen device names to the screens:
            foreach (Screen screen in Screen.AllScreens)
            {
                screenDeviceNameToScreenMap.Add(screen.DeviceName, screen);
            }

            List<String> initialArrangement = GetAutoArrangedLTRScreenDeviceNames();
            
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

                // Draw numbers on screens
                using (Graphics formGraphics = form.CreateGraphics())
                {
                    formGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    // Determine the number to display based on the device name
                    int displayIndex = screen.DeviceName.LastIndexOf("DISPLAY");
                    int displayNum = Int32.Parse(screen.DeviceName.Substring(displayIndex + "DISPLAY".Length));

                    // Create the font and brush for drawing
                    //using (Font font = new Font("Gill Sans MT", 256))
                    using (Font measureFont = new Font(SystemFonts.CaptionFont.FontFamily, screenBounds.Height / 6, FontStyle.Regular))
                    {
                        // Get the size of the string when drawn with the given font
                        SizeF stringSize = formGraphics.MeasureString(currentScreenId.ToString(), measureFont);

                        // Calculate the top-left point of the string to draw it centered in the form
                        float x = (form.Width - stringSize.Width) / 2;
                        float y = ((form.Height - stringSize.Height) / 2) - (stringSize.Height / 10);

                        if (screen.Primary)
                        {
                            primarySceenIdTextY = (int)y;
                        }

                        Font paintFont = (Font)measureFont.Clone();

                        form.Paint += (s, e) =>
                        {
                            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                            // Draw the string
                            using (Brush brush = new SolidBrush(Color.Black))
                            {
                                e.Graphics.DrawString(currentScreenId.ToString(), paintFont, brush, new PointF(x, y));
                            }
                        };
                    }
                };

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

            Func<int, Form> deselectScreenIdFormByScreenIndex = (screenIdx) => {
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

            Func<RoundedTextBox, Form> unselectScreenIdFormByTextboxValue = (input) => {
                if (input.Text.Length > 0)
                {
                    return deselectScreenIdFormByScreenIndex(Int32.Parse(input.Text));
                }

                return null;
            };

            if (primaryForm != null)
            {

                primaryForm.Closed += (s, e) =>
                {
                    screenIdForms.ForEach(f => {
                        if (f != primaryForm)
                        {
                            f.Close();
                            f.Dispose();
                        }
                    });
                };

                RoundedTextBox firstTextBox = null;

                RoundedTextBox previouslyAddedTextBox = null;

                int inputFontSize = primaryForm.Height / 14;

                int tbWidth = (int)Math.Round(inputFontSize*2.1);
                int tbMargin = (int)Math.Round(tbWidth * 0.2);

                int screenCount = Screen.AllScreens.Count();
                int intTbTotalWidth = ((tbWidth + tbMargin) * screenCount) - tbMargin;
                int startLeft = ((primaryForm.Width - (intTbTotalWidth)) / 2);

                int textboxIndex = 0;

                Screen[] allScreens = Screen.AllScreens;
                int maxScreens = allScreens.Count();

                foreach (Screen screen in allScreens)
                {

                    Point textboxLoc = new Point(startLeft + (textboxIndex * (tbWidth + tbMargin)), primarySceenIdTextY - inputFontSize);

                    textboxLoc.Offset(0, -((textboxLoc.Y) / 2));

                    RoundedTextBox textBox = new()
                    {
                        Font = new Font(SystemFonts.CaptionFont.FontFamily, inputFontSize, FontStyle.Regular),
                        Width = tbWidth, // adjust as needed
                        Location = textboxLoc, // adjust as needed
                        MaxLength = 1,
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White,
                        ForeColor = Color.Black,
                        Tag = textboxIndex,
                        TextAlign = HorizontalAlignment.Center,
                        Margin = new Padding(5, 0, 0, 20),
                    };

                    screenIdTextBoxList.Add(textBox);

                    RoundedTextBox previousTextBox = previouslyAddedTextBox;

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

                            if (typedScreenIndex < 1 || typedScreenIndex > maxScreens)
                            {
                                e.Handled = true; // Cancel the character
                                return;
                            }


                            for (int i = 0; i < screenIdTextBoxList.Count; i++)
                            {
                                if (i != (int)((RoundedTextBox)s).Tag)
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
                        System.Diagnostics.Debug.WriteLine("Got keyup");

                        previousTextBox.KeyUp += (s, e) =>
                        {
                            if (e.KeyCode != Keys.Back)
                            {
                                if (((RoundedTextBox)s).Text.Trim().Length > 0) {
                                    textBox.Focus();
                                }

                                System.Diagnostics.Debug.WriteLine("Next on keyup");
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


                int scaleFontSize = primaryForm.Height / 64;
                string scaleText = Properties.Resources.ScaleDisplays;

                ScalableCheckBox autoScaleDisplaysCheckBox = new()
                {
                    Font = new Font(SystemFonts.CaptionFont.FontFamily, scaleFontSize, FontStyle.Regular),
                    Tag = textboxIndex,
                    Text = scaleText,
                    Margin = new Padding(0, 0, 0, 0),
                };

                autoScaleDisplaysCheckBox.SetSizeToRequired();

                Point scaleDisplaysCheckBoxLoc = new Point((primaryForm.Width - autoScaleDisplaysCheckBox.Size.Width) / 2, firstTextBox.Location.Y);
                //scaleDisplaysCheckBoxLoc.Offset(0, firstTextBox.Height+10);
                scaleDisplaysCheckBoxLoc.Offset(0, -(autoScaleDisplaysCheckBox.Height + 20));

                autoScaleDisplaysCheckBox.Location = scaleDisplaysCheckBoxLoc;
                primaryForm.Controls.Add(autoScaleDisplaysCheckBox);

                // Calculate the top-left point of the string to draw it centered in the form
                //float formCenterX = (form.Width - stringSize.Width) / 2;
                //float formCenterY = ((form.Height - stringSize.Height) / 2) - (stringSize.Height / 8);
 
                RoundedTextBox lastTextBox = previouslyAddedTextBox;

                if (firstTextBox != null)
                {
                    firstTextBox.Focus();
                }

                if (lastTextBox != null)
                {
                    lastTextBox.KeyUp += (s, e) =>
                    {
                        if (((RoundedTextBox)s).Text.Trim().Length == 0)
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

                        ArrangeLTRWithAutoPrimary(userOrderedDeviceNames);

                        if (autoScaleDisplaysCheckBox.IsChecked == true) {
                            autoscaleExternalDisplays();
                        } else
                        {
                            resetScalingForExternalDisplays();
                        }

                        // Sleep 100ms
                        System.Threading.Thread.Sleep(100);

                        primaryForm.Close();
                        
                    };
                }

                screenIdForms.ForEach(f => { f.Show(); });

                primaryForm.BringToFront();
                primaryForm.Focus();
                return primaryForm;
            }

            return null;
        }

        public static void autoscaleExternalDisplays()
        {
            List<DisplayInfo> displayInfoList = DisplayInfo.GetDisplayInfoForAllConnectedDisplayDevices();

            foreach (var displayInfo in displayInfoList)
            {
                if (displayInfo.Internal)
                {
                    continue;
                }

                Extern.Displays.SetDpiScaling(displayInfo.PnpDeviceId, 125);
            }
        }

        public static void resetScalingForExternalDisplays()
        {
            List<DisplayInfo> displayInfoList = DisplayInfo.GetDisplayInfoForAllConnectedDisplayDevices();
            foreach (var displayInfo in displayInfoList)
            {
                if (displayInfo.Internal)
                {
                    continue;
                }
                Extern.Displays.SetDpiScaling(displayInfo.PnpDeviceId, 100);
            }
        }

        public static void ArrangeLTRWithAutoPrimary(List<String> screenDeviceNamesLTR) {
            List<DisplayInfo> displayInfoList = DisplayInfo.GetDisplayInfoForAllConnectedDisplayDevices();

            if (displayInfoList.Count == 1 && displayInfoList[0].Internal)
            {
                Extern.Displays.Arrange(screenDeviceNamesLTR[0], Array.Empty<String>(), Array.Empty<String>());
                return;
            }

            int primaryDisplayIndex = 0;

            foreach (var deviceName in screenDeviceNamesLTR)
            {
                bool displayInternal = false;

                foreach(var displayInfo in displayInfoList)
                {
                    if (displayInfo.DeviceName.ToUpper().Equals(deviceName.ToUpper())) {
                        if (displayInfo.Internal)
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
