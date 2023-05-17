using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using KhBroDisplaySetup.Extern;
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

        public static List<Dictionary<string, string>> GetMonitorInfoForAllConnectedDisplayDevices()
        {
            var displayPnpIdToDisplayName = Extern.Displays.GetDisplayDeviceIdToDisplayDeviceNameMapping();
            foreach (KeyValuePair<string, string> kvp in displayPnpIdToDisplayName)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                System.Diagnostics.Debug.WriteLine("{0}={1}", kvp.Key, kvp.Value);
            }

            var wmiMonitorDisplayDevices = new List<Dictionary<string, string>>();

            var scope = new ManagementScope("root\\wmi");
            var query = new ObjectQuery("SELECT * FROM WmiMonitorID");
            var searcher = new ManagementObjectSearcher(scope, query);
            var monitorInfoList = searcher.Get();

            Dictionary<string, Extern.DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY> displayOutputTechnologies = Extern.Displays.GetVideoOutputTechnologyByDevicePathMap();

            foreach(var tech in displayOutputTechnologies)
            {
                System.Diagnostics.Debug.WriteLine("{0}={1}", tech.Key, tech.Value);
            }

            foreach (ManagementObject monitor in monitorInfoList)
            {
                string name = DecodeName(monitor["UserFriendlyName"] as ushort[]);
                string serial = DecodeName(monitor["SerialNumberID"] as ushort[]);
                string productCodeID = DecodeName(monitor["ProductCodeID"] as ushort[]);
                string manufacturerName = DecodeName(monitor["ManufacturerName"] as ushort[]);
                string instanceName = (string)monitor["InstanceName"];
                string monitorPnpDeviceId = ((string)monitor["InstanceName"]).Split('_')[0].ToUpper();

                string displayProduct = monitorPnpDeviceId.Split('\\')[1];

                string matchUser32DeviceId = monitorPnpDeviceId.Replace("\\", "#");
                matchUser32DeviceId = "\\\\?\\" + matchUser32DeviceId;

                

                String outputTechnology = "Unknown";
                String internalMonitor = "Unknown";
                foreach (var tech in displayOutputTechnologies)
                {
                    System.Diagnostics.Debug.WriteLine("{0} contains {1}", tech.Key.ToUpper(), monitorPnpDeviceId.Replace("\\", "#"));
                    if (tech.Key.ToUpper().Contains(monitorPnpDeviceId.Replace("\\", "#")))
                    {
                        outputTechnology = tech.Value.ToString();
                        if (tech.Value == DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL)
                        {
                            internalMonitor = "Yes";
                        } else {
                            internalMonitor = "No";
                        }
                        break;
                    }
                }

                var limitedMonitorInfo = new Dictionary<string, string>
                {
                    { "Name", name },
                    { "Serial", serial },
                    { "ProductCodeID", productCodeID },
                    { "Manufacturer", manufacturerName },
                    { "PnpDeviceId", monitorPnpDeviceId },
                    { "InstanceName", instanceName },
                    { "VideoOutputTechnology", outputTechnology },
                    { "Internal", internalMonitor }
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

                String deviceName = displayPnpIdToDisplayName[user32DeviceId];

                limitedMonitorInfo.Add("DeviceName", deviceName);

                DEVMODE currentDevMode = Extern.Displays.GetCurrentDisplayMode(deviceName);
                limitedMonitorInfo.Add("Bounds", currentDevMode.dmPosition.x + ", " + currentDevMode.dmPosition.y + ", " + currentDevMode.dmPelsWidth + ", " + currentDevMode.dmPelsHeight);

                Extern.DEVMODE optimalDevMode = Extern.Displays.GetOptimalDisplayMode(deviceName);

                limitedMonitorInfo.Add("Optimal resolution", optimalDevMode.dmPelsWidth + "x" + optimalDevMode.dmPelsHeight);

                foreach (KeyValuePair<string, string> kvp in limitedMonitorInfo)
                {
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
