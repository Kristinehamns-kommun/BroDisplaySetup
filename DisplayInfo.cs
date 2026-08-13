using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using BroDisplaySetup.DPI;
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
        public DPIScalingInfo DpiScalingInfo { get; set; }

        // Physical screen dimensions in cm, from EDID via WmiMonitorBasicDisplayParams. 0x0 if unreported.
        public int PhysicalWidthCm { get; set; }
        public int PhysicalHeightCm { get; set; }

        public double DiagonalInches =>
            (PhysicalWidthCm == 0 && PhysicalHeightCm == 0)
                ? 0
                : Math.Sqrt(PhysicalWidthCm * PhysicalWidthCm + PhysicalHeightCm * PhysicalHeightCm) / 2.54;

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
                   $"OptimalResolution: {OptimalResolution.Width} x {OptimalResolution.Height}{Environment.NewLine}" +
                   $"PhysicalSize: {PhysicalWidthCm} x {PhysicalHeightCm} cm ({DiagonalInches:0.#}\" diagonal){Environment.NewLine}" +
                   $"DpiScalingInfo: {DpiScalingInfo.Current}% (min: {DpiScalingInfo.Minimum}%, max: {DpiScalingInfo.Maximum}%, recommended: {DpiScalingInfo.Recommended}%)";
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

            foreach (var tech in displayOutputTechnologies)
            {
                System.Diagnostics.Debug.WriteLine("{0}={1}", tech.Key, tech.Value);
            }

            IEnumerable<KeyValuePair<string, DPIScalingInfo>> dpiScales = DPI.DPIHelper.GetDpiScalingInfoByDevicePathMap();

            foreach (var dpi in dpiScales)
            {
                System.Diagnostics.Debug.WriteLine("{0}={1} DPI (min:{2}, max:{3})", dpi.Key, dpi.Value.Current, dpi.Value.Minimum, dpi.Value.Maximum);
            }

            // Physical screen dimensions (cm), from EDID, keyed by the same InstanceName as WmiMonitorID.
            var physicalSizeQuery = new ObjectQuery("SELECT * FROM WmiMonitorBasicDisplayParams");
            var physicalSizeSearcher = new ManagementObjectSearcher(scope, physicalSizeQuery);
            var physicalSizeByInstanceName = new Dictionary<string, (int WidthCm, int HeightCm)>();
            foreach (ManagementObject basicParams in physicalSizeSearcher.Get())
            {
                string paramsInstanceName = (string)basicParams["InstanceName"];
                int widthCm = Convert.ToInt32(basicParams["MaxHorizontalImageSize"]);
                int heightCm = Convert.ToInt32(basicParams["MaxVerticalImageSize"]);
                physicalSizeByInstanceName[paramsInstanceName] = (widthCm, heightCm);
            }

            foreach (ManagementObject monitor in wmiMonitorCollection)
            {
                string userFriendlyName = DecodeName(monitor["UserFriendlyName"] as ushort[]);
                string serial = DecodeName(monitor["SerialNumberID"] as ushort[]);
                string productCodeID = DecodeName(monitor["ProductCodeID"] as ushort[]);
                string manufacturerName = DecodeName(monitor["ManufacturerName"] as ushort[]);
                string instanceName = (string)monitor["InstanceName"];
                string monitorPnpDeviceId = ((string)monitor["InstanceName"]).Split('_')[0].ToUpper();
                DPIScalingInfo deviceDPIScalingInfo = new DPIScalingInfo();

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

                foreach(var dpi in dpiScales)
                {
                    System.Diagnostics.Debug.WriteLine("{0} contains {1}", dpi.Key.ToUpper(), monitorPnpDeviceId.Replace("\\", "#"));
                    if (dpi.Key.ToUpper().Contains(monitorPnpDeviceId.Replace("\\", "#")))
                    {
                        System.Diagnostics.Debug.WriteLine("Found DPI scaling for {0}", monitorPnpDeviceId);
                        System.Diagnostics.Debug.WriteLine("DPI scaling is {0}", dpi.Value.Current);
                        deviceDPIScalingInfo = dpi.Value;
                        break;
                    }
                }   

                physicalSizeByInstanceName.TryGetValue(instanceName, out var physicalSize);

                var displayInfo = new DisplayInfo
                {
                    UserFriendlyName = userFriendlyName,
                    Serial = serial,
                    ProductCodeID = productCodeID,
                    Manufacturer = manufacturerName,
                    PnpDeviceId = monitorPnpDeviceId,
                    InstanceName = instanceName,
                    VideoOutputTechnology = outputTechnology,
                    Internal = internalMonitor,
                    DpiScalingInfo = deviceDPIScalingInfo,
                    PhysicalWidthCm = physicalSize.WidthCm,
                    PhysicalHeightCm = physicalSize.HeightCm
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
