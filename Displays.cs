﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KhBroDisplaySetup
{
    class Displays
    {
        public static void AutoArrange()
        {
            Extern.Displays.SwitchToExtendModeIfClone();

            Screen[] screens = Screen.AllScreens;
            List<string> screenDeviceNames = new();

            foreach (Screen screen in screens)
            {
                screenDeviceNames.Add(screen.DeviceName);
            }

            string primaryDisplayName = null;
            List<string> leftDisplayNames = new();
            List<string> rightDisplayNames = new();

            if (screenDeviceNames.Count == 1)
            {
                primaryDisplayName = screenDeviceNames[0];
                // Abort script, nothing to do:
                return;
            }
            else if (screenDeviceNames.Count > 1)
            {
                // Assume the laptop is to the left
                leftDisplayNames.Add(screenDeviceNames[0]);
                screenDeviceNames.RemoveAt(0); // Remove the first screen from the list

                // The primary screen is the second screen to the right
                primaryDisplayName = screenDeviceNames[0];
                screenDeviceNames.RemoveAt(0);

                // Check that screenDeviceNames is not empty
                if (screenDeviceNames.Count > 0)
                {
                    // Set rightDisplayNames to the rest of the screens
                    rightDisplayNames.AddRange(screenDeviceNames);
                }
            }

            Extern.Displays.Arrange(primaryDisplayName, leftDisplayNames.ToArray(), rightDisplayNames.ToArray());
        }
    }
}
