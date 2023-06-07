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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //private void autoArrangeDisplays_Click(object sender, EventArgs e)
        //{
        //   
        //}

        private void btnScreenInfo_Click(object sender, EventArgs e)
        {
            //displayInfoRetriever.GetWmiMonitorsWithDisplayDeviceName();
            ScreenInfoForm screenInfoForm = new ScreenInfoForm();
            screenInfoForm.Show();
        }

        private void btnManualArrangeDisplays_Click(object sender, EventArgs e)
        {
            Displays.ArrangeManuallyFromLTRWithAutoResolution();
        }

        private void btnAutoArrangeDisplays_Click(object sender, EventArgs e)
        {
            Displays.ArrangeAutomaticallyFromLTRWithAutoResolution();
        }
    }
}
