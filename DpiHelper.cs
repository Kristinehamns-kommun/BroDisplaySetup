using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace BroDisplaySetup
{
    namespace DPI
    {
        public class DPIConstants
        {
            /*
             * OS reports DPI scaling values in relative terms, and not absolute terms.
             * eg. if current DPI value is 250%, and recommended value is 200%, then
             * OS will give us integer 2 for DPI scaling value (starting from recommended
             * DPI scaling move 2 steps to the right in this list).
             * values observed (and extrapolated) from system settings app (immersive control panel).
             */
            public static readonly uint[] DpiVals = { 100, 125, 150, 175, 200, 225, 250, 300, 350, 400, 450, 500 };
        }

        public struct DPIScalingInfo
        {
            public uint Minimum { get; set; }
            public uint Maximum { get; set; }
            public uint Current { get; set; }
            public uint Recommended { get; set; }
            public bool InitDone { get; set; }

            public DPIScalingInfo()
            {
                Minimum = 100;
                Maximum = 100;
                Current = 100;
                Recommended = 100;
                InitDone = false;
            }

            public DPIScalingInfo(Extern.DISPLAYCONFIG_SOURCE_DPI_SCALE_GET fromDpiScaleGetResult)
            {
                Minimum = 100;

                int minAbs = Math.Abs((int)fromDpiScaleGetResult.minScaleRel);
                int getVal = minAbs + fromDpiScaleGetResult.curScaleRel;
                if (getVal < 0 || getVal >= DPIConstants.DpiVals.Length)
                {
                    //throw new ArgumentOutOfRangeException("fromDpiScaleGetResult", "Invalid DPI scaling value");
                    Current = DPIConstants.DpiVals[0];
                } else
                {
                    Current = DPIConstants.DpiVals[minAbs + fromDpiScaleGetResult.curScaleRel];
                }
                
                Recommended = DPIConstants.DpiVals[minAbs];
                Maximum = DPIConstants.DpiVals[minAbs + fromDpiScaleGetResult.maxScaleRel];
                InitDone = true;
            }
        }

        public class DPIHelper
        {
            public static IEnumerable<KeyValuePair<string, DPIScalingInfo>> GetDpiScalingInfoByDevicePathMap()
            {
                Dictionary<string, Extern.DISPLAYCONFIG_SOURCE_DPI_SCALE_GET> dpiScales = Extern.Displays.GetDPISettingByDevicePathMap();

                foreach (var dpi in dpiScales)
                {
                    yield return new KeyValuePair<string, DPIScalingInfo>(dpi.Key, new DPIScalingInfo(dpi.Value));
                }
            }
        }
    }
}
