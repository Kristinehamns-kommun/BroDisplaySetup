using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KhBroDisplaySetup
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            // Retrieve the program name
            string programName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            // Get the assembly where the attribute is defined (usually the executing assembly)
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the AssemblyInformationalVersionAttribute
            AssemblyInformationalVersionAttribute versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            AssemblyCopyrightAttribute copyrightAttribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();

            // Access the attribute value
            string programVersion = versionAttribute?.InformationalVersion;

            // Access the copyright value
            string copyright = copyrightAttribute?.Copyright;

            // Set the form's title with program name and version in parentheses
            this.Text = $"Om {programName} ({programVersion})";

            GplV3TextBox.Text = $"{programName} ({programVersion})\r\n{copyright}\r\n\r\n" + Properties.Resources.GPLv3;
            GplV3TextBox.SelectionLength = 0;
            GplV3TextBox.Select(0, 0);
            okButton.Focus();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
