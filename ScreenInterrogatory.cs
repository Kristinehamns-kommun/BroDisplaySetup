using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BroDisplaySetup
{
    public static class ScreenInterrogatory
    {
        public const int ERROR_SUCCESS = 0;

        #region DLL-Imports

        [DllImport("user32.dll")]
        public static extern int GetDisplayConfigBufferSizes(
            Extern.QUERY_DEVICE_CONFIG_FLAGS flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

        [DllImport("user32.dll")]
        public static extern int QueryDisplayConfig(
            Extern.QUERY_DEVICE_CONFIG_FLAGS flags,
            ref uint numPathArrayElements, [Out] Extern.DISPLAYCONFIG_PATH_INFO[] PathInfoArray,
            ref uint numModeInfoArrayElements, [Out] Extern.DISPLAYCONFIG_MODE_INFO[] ModeInfoArray,
            IntPtr currentTopologyId
            );

        [DllImport("user32.dll")]
        public static extern int DisplayConfigGetDeviceInfo(ref Extern.DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName);

        #endregion

        private static string MonitorFriendlyName(Extern.LUID adapterId, uint targetId)
        {
            var deviceName = new Extern.DISPLAYCONFIG_TARGET_DEVICE_NAME
            {
                header =
                {
                    size = (uint)Marshal.SizeOf(typeof (Extern.DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                    adapterId = adapterId,
                    id = targetId,
                    type = Extern.DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                }
            };
            var error = DisplayConfigGetDeviceInfo(ref deviceName);
            if (error != ERROR_SUCCESS)
                throw new Win32Exception(error);
            return deviceName.monitorFriendlyDeviceName;
        }

        public static IEnumerable<string> GetAllMonitorsFriendlyNames()
        {
            uint pathCount, modeCount;
            var error = GetDisplayConfigBufferSizes(Extern.QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
            if (error != ERROR_SUCCESS)
                throw new Win32Exception(error);

            var displayPaths = new Extern.DISPLAYCONFIG_PATH_INFO[pathCount];
            var displayModes = new Extern.DISPLAYCONFIG_MODE_INFO[modeCount];
            error = QueryDisplayConfig(Extern.QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
                ref pathCount, displayPaths, ref modeCount, displayModes, IntPtr.Zero);
            if (error != ERROR_SUCCESS)
                throw new Win32Exception(error);

            for (var i = 0; i < modeCount; i++)
                if (displayModes[i].infoType == Extern.DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET)
                    yield return MonitorFriendlyName(displayModes[i].adapterId, displayModes[i].id);
        }

        public static string DeviceFriendlyName(this Screen screen)
        {
            var allFriendlyNames = GetAllMonitorsFriendlyNames();
            for (var index = 0; index < Screen.AllScreens.Length; index++)
                if (Equals(screen, Screen.AllScreens[index]))
                    return allFriendlyNames.ToArray()[index];
            return null;
        }

    }
}
