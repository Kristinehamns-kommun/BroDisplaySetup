using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KhBroDisplaySetup
{
    public partial class ScreenInfoForm : Form
    {
        public ScreenInfoForm()
        {
            InitializeComponent();
        }

        private void ScreenInfoForm_Load(object sender, EventArgs e)
        {
            List<Dictionary<string, string>> displayWmi = MonitorInfoRetriever.GetWmiMonitorsWithDisplayDeviceName();

            List<string> screenInfo = new();

            foreach (var disp in displayWmi)
            {
                foreach (KeyValuePair<string, string> kvp in disp)
                {
                    //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                    screenInfo.Add(kvp.Key + ": \"" + kvp.Value + "\"");
                }

                screenInfo.Add("--------------------------" + Environment.NewLine);
            }

            txtScreenInfo.Text = string.Join(Environment.NewLine, screenInfo);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}
