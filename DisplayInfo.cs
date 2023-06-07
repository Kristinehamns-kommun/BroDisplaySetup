using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using BroDisplaySetup.Extern;
using Microsoft.Win32;

namespace BroDisplaySetup
{
    class DisplayInfo
    {
        public string UserFriendlyName { get; set; }
        public string Serial { get; set; }
        public string ProductCodeID { get; set; }
        public string Manufacturer { get; set; }
        public string PnpDeviceId { get; set; }
        public string InstanceName { get; set; }
        public string VideoOutputTechnology { get; set; }
        public bool Internal { get; set; }
        public string DeviceName { get; set; }
        public Rectangle Bounds { get; set; }
        public Size OptimalResolution { get; set; }

        public override string ToString()
        {
            string internalStringValue = Internal ? "Yes" : "No";

            return $"UserFriendlyName: {UserFriendlyName}{Environment.NewLine}" +
                   $"DeviceName: {DeviceName}{Environment.NewLine}" +
                   $"Serial: {Serial}{Environment.NewLine}" +
                   $"ProductCodeID: {ProductCodeID}{Environment.NewLine}" +
                   $"Manufacturer: {Manufacturer}{Environment.NewLine}" +
                   $"PnpDeviceId: {PnpDeviceId}{Environment.NewLine}" +
                   $"InstanceName: {InstanceName}{Environment.NewLine}" +
                   $"VideoOutputTechnology: {VideoOutputTechnology}{Environment.NewLine}" +
                   $"Internal: {internalStringValue}{Environment.NewLine}" +
                   $"Bounds: {Bounds}{Environment.NewLine}" +
                   $"OptimalResolution: {OptimalResolution.Width} x {OptimalResolution.Height}";
        }

        public static List<DisplayInfo> GetDisplayInfoForAllConnectedDisplayDevices()
        {
            var displayPnpIdToDisplayName = Extern.Displays.GetDisplayDeviceIdToDisplayDeviceNameMapping();
            foreach (KeyValuePair<string, string> kvp in displayPnpIdToDisplayName)
            {
                System.Diagnostics.Debug.WriteLine("{0}={1}", kvp.Key, kvp.Value);
            }

            var displayInfoList = new List<DisplayInfo>();

            var scope = new ManagementScope("root\\wmi");
            var query = new ObjectQuery("SELECT * FROM WmiMonitorID");
            var searcher = new ManagementObjectSearcher(scope, query);
            var wmiMonitorCollection = searcher.Get();

            Dictionary<string, Extern.DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY> displayOutputTechnologies = Extern.Displays.GetVideoOutputTechnologyByDevicePathMap();

            foreach(var tech in displayOutputTechnologies)
            {
                System.Diagnostics.Debug.WriteLine("{0}={1}", tech.Key, tech.Value);
            }

            foreach (ManagementObject monitor in wmiMonitorCollection)
            {
                string userFriendlyName = DecodeName(monitor["UserFriendlyName"] as ushort[]);
                string serial = DecodeName(monitor["SerialNumberID"] as ushort[]);
                string productCodeID = DecodeName(monitor["ProductCodeID"] as ushort[]);
                string manufacturerName = DecodeName(monitor["ManufacturerName"] as ushort[]);
                string instanceName = (string)monitor["InstanceName"];
                string monitorPnpDeviceId = ((string)monitor["InstanceName"]).Split('_')[0].ToUpper();

                string displayProduct = monitorPnpDeviceId.Split('\\')[1];

                string matchUser32DeviceId = monitorPnpDeviceId.Replace("\\", "#");
                matchUser32DeviceId = "\\\\?\\" + matchUser32DeviceId;

                String outputTechnology = "Unknown";
                bool internalMonitor = false;
                foreach (var tech in displayOutputTechnologies)
                {
                    System.Diagnostics.Debug.WriteLine("{0} contains {1}", tech.Key.ToUpper(), monitorPnpDeviceId.Replace("\\", "#"));
                    if (tech.Key.ToUpper().Contains(monitorPnpDeviceId.Replace("\\", "#")))
                    {
                        outputTechnology = tech.Value.ToString();
                        if (tech.Value == DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY.DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL)
                        {
                            internalMonitor = true;
                        } else {
                            internalMonitor = false;
                        }
                        break;
                    }
                }

                var displayInfo = new DisplayInfo
                {
                    UserFriendlyName = userFriendlyName,
                    Serial = serial,
                    ProductCodeID = productCodeID,
                    Manufacturer = manufacturerName,
                    PnpDeviceId = monitorPnpDeviceId,
                    InstanceName = instanceName,
                    VideoOutputTechnology = outputTechnology,
                    Internal = internalMonitor
                };


                System.Diagnostics.Debug.WriteLine("Try matching pnp device id to known displays {0}", matchUser32DeviceId);

                string user32DeviceId = displayPnpIdToDisplayName.Keys.FirstOrDefault(x => x.ToUpper().StartsWith(matchUser32DeviceId));

                System.Diagnostics.Debug.WriteLine("Found matching device id:" + user32DeviceId);

                String deviceName = displayPnpIdToDisplayName[user32DeviceId];

                displayInfo.DeviceName = deviceName;

                DEVMODE currentDevMode = Extern.Displays.GetCurrentDisplayMode(deviceName);
                displayInfo.Bounds = new Rectangle(currentDevMode.dmPosition.x, currentDevMode.dmPosition.y, (int)currentDevMode.dmPelsWidth, (int)currentDevMode.dmPelsHeight);

                Extern.DEVMODE optimalDevMode = Extern.Displays.GetOptimalDisplayMode(deviceName);

                displayInfo.OptimalResolution =  new Size((int)optimalDevMode.dmPelsWidth, (int)optimalDevMode.dmPelsHeight);

                foreach (DisplayInfo d in displayInfoList)
                {
                    System.Diagnostics.Debug.Write("DisplayInfo: " + d.ToString());
                }

                displayInfoList.Add(displayInfo);
            }

            return displayInfoList;
        }

        private static string DecodeName(ushort[] input)
        {
            if (input == null)
                return null;

            return new string(input.Where(x => x != 0).Select(x => (char)x).ToArray());
        }
    }
}
