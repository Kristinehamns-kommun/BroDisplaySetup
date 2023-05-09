using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Microsoft.Win32;

namespace KhBroDisplaySetup
{
    class MonitorInfoRetriever
    {
        //class MonitorInfoInsertionOrderComparer : IComparer<string>
        //{
        //    public int Compare(string x, string y)
        //    {
        //        return 0;
        //    }
        //}

        public static List<Dictionary<string, string>> GetWmiMonitorsWithDisplayDeviceName()
        {
            var displayPnpIdToDisplayName = Extern.Displays.GetDisplayDeviceIdToDisplayDeviceNameMapping();
            foreach (KeyValuePair<string, string> kvp in displayPnpIdToDisplayName)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                System.Diagnostics.Debug.WriteLine("{0}={1}", kvp.Key, kvp.Value);
            }

            //var displayNameToWmiMonitorInfo = new SortedDictionary<string, Dictionary<string, string>>(new MonitorInfoInsertionOrderComparer());
            var wmiMonitorDisplayDevices = new List<Dictionary<string, string>>();

            var scope = new ManagementScope("root\\wmi");
            var query = new ObjectQuery("SELECT * FROM WmiMonitorID");
            var searcher = new ManagementObjectSearcher(scope, query);
            var monitorInfoList = searcher.Get();

            //string regPath = @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers\Configuration";
            //var regKeys = Registry.LocalMachine.OpenSubKey(regPath).GetSubKeyNames();

            foreach (ManagementObject monitor in monitorInfoList)
            {
                string name = DecodeName(monitor["UserFriendlyName"] as ushort[]);
                string serial = DecodeName(monitor["SerialNumberID"] as ushort[]);
                string productCodeID = DecodeName(monitor["ProductCodeID"] as ushort[]);
                string monitorPnpDeviceId = ((string)monitor["InstanceName"]).Split('_')[0].ToUpper();

                string displayProduct = monitorPnpDeviceId.Split('\\')[1];

                string matchUser32DeviceId = monitorPnpDeviceId.Replace("\\", "#");
                matchUser32DeviceId = "\\\\?\\" + matchUser32DeviceId;

             
                var limitedMonitorInfo = new Dictionary<string, string>
                {
                    { "Name", name },
                    { "Serial", serial },
                    { "ProductCodeID", productCodeID },
                    { "PnpDeviceId", monitorPnpDeviceId }
                };

                //foreach (KeyValuePair<string, string> kvp in limitedMonitorInfo)
                //{
                //    //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                //    System.Diagnostics.Debug.WriteLine("{0}={1}", kvp.Key, kvp.Value);
                //}

                System.Diagnostics.Debug.WriteLine("Try matching pnp device id to known displays {0}", matchUser32DeviceId);

                //foreach (KeyValuePair<string, string> kvp in displayPnpIdToDisplayName)
                //{
                //    //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                //    System.Diagnostics.Debug.WriteLine("{0}={1}", kvp.Key, kvp.Value);
                //    System.Diagnostics.Debug.WriteLine("'{0}'=='{1}' ? {2}", kvp.Key, matchUser32DeviceId, kvp.Key.StartsWith(matchUser32DeviceId));
                //}

                string user32DeviceId = displayPnpIdToDisplayName.Keys.FirstOrDefault(x => x.ToUpper().StartsWith(matchUser32DeviceId));

                limitedMonitorInfo.Add("DeviceName", displayPnpIdToDisplayName[user32DeviceId]);

                foreach (KeyValuePair<string, string> kvp in limitedMonitorInfo)
                {
                    //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                    System.Diagnostics.Debug.WriteLine("{0}={1}", kvp.Key, kvp.Value);
                }

                System.Diagnostics.Debug.WriteLine("Found matching device id:" + user32DeviceId);
                wmiMonitorDisplayDevices.Add(limitedMonitorInfo);

                //regKeys = regKeys.Where(x => x.IndexOf(displayProduct + serial, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
            }

            return wmiMonitorDisplayDevices;
        }

        private static string DecodeName(ushort[] input)
        {
            if (input == null)
                return null;

            return new string(input.Where(x => x != 0).Select(x => (char)x).ToArray());
        }
    }
}
