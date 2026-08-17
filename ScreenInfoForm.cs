using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BroDisplaySetup
{
    public partial class ScreenInfoForm : Form
    {
        public ScreenInfoForm()
        {
            InitializeComponent();
        }

        private void ScreenInfoForm_Load(object sender, EventArgs e)
        {
            List<DisplayInfo> displayInfoList = DisplayInfo.GetDisplayInfoForAllConnectedDisplayDevices();

            List<string> screenInfo = new();

            // Condensed summary first - just the resolution-selection outcome per display, so this
            // can be copied off the top of the box without scrolling past the full per-display dump
            // below (which is a lot to sift through when only the resolution pick is in question).
            screenInfo.Add("=== Resolution diagnostics summary ===");
            foreach (var displayInfo in displayInfoList)
            {
                screenInfo.Add($"{displayInfo.UserFriendlyName} ({displayInfo.DeviceName}): picked {displayInfo.OptimalResolution.Width}x{displayInfo.OptimalResolution.Height} - {displayInfo.OptimalResolutionDiagnostics}");
            }
            screenInfo.Add("=== Full details below ===");
            screenInfo.Add("--------------------------");

            if (!string.IsNullOrWhiteSpace(Displays.ConferenceRoomCandidateSerial))
            {
                screenInfo.Add($"Konferensrumsläge: {(Displays.ConferenceRoomModeActive ? "Ja" : "Nej")}");
            }
            screenInfo.Add("--------------------------");
            foreach (var displayInfo in displayInfoList)
            {
                screenInfo.Add(displayInfo.ToString());
                screenInfo.Add("--------------------------");
            }

            txtScreenInfo.Text = string.Join(Environment.NewLine, screenInfo);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}
