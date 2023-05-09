using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;

namespace KhBroDisplaySetup 
{ 
    namespace Extern
    {
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            [FieldOffset(0)]
            public string dmDeviceName;

            [FieldOffset(32)]
            public ushort dmSpecVersion;

            [FieldOffset(34)]
            public ushort dmDriverVersion;

            [FieldOffset(36)]
            public ushort dmSize;

            [FieldOffset(38)]
            public ushort dmDriverExtra;

            [FieldOffset(40)]
            public uint dmFields;

            [FieldOffset(44)]
            public NestedStruct nestedStruct;

            [FieldOffset(44)]
            public POINTL dmPosition;

            [FieldOffset(44)]
            public NestedStruct2 nestedStruct2;

            [FieldOffset(60)]
            public short dmColor;

            [FieldOffset(62)]
            public short dmDuplex;

            [FieldOffset(64)]
            public short dmYResolution;

            [FieldOffset(66)]
            public short dmTTOption;

            [FieldOffset(68)]
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            [FieldOffset(72)]
            public string dmFormName;

            [FieldOffset(102)]
            public ushort dmLogPixels;

            [FieldOffset(104)]
            public uint dmBitsPerPel;

            [FieldOffset(108)]
            public uint dmPelsWidth;

            [FieldOffset(112)]
            public uint dmPelsHeight;

            [FieldOffset(116)]
            public uint dmDisplayFlags;

            [FieldOffset(116)]
            public uint dmNup;

            [FieldOffset(120)]
            public uint dmDisplayFrequency;

            [FieldOffset(124)]
            public uint dmICMMethod;

            [FieldOffset(128)]
            public uint dmICMIntent;

            [FieldOffset(132)]
            public uint dmMediaType;

            [FieldOffset(136)]
            public uint dmDitherType;

            [FieldOffset(140)]
            public uint dmReserved1;

            [FieldOffset(144)]
            public uint dmReserved2;

            [FieldOffset(148)]
            public uint dmPanningWidth;

            [FieldOffset(152)]
            public uint dmPanningHeight;

            [StructLayout(LayoutKind.Sequential)]
            public struct NestedStruct
            {
                public short dmOrientation;
                public short dmPaperSize;
                public short dmPaperLength;
                public short dmPaperWidth;
                public short dmScale;
                public short dmCopies;
                public short dmDefaultSource;
                public short dmPrintQuality;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct POINTL
            {
                public int x;
                public int y;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct NestedStruct2
            {
                public POINTL dmPosition;
                public uint dmDisplayOrientation;
                public uint dmDisplayFixedOutput;
            }
        }

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [Flags]
        public enum SdcFlags : uint
        {
            SDC_TOPOLOGY_INTERNAL = 0x00000001,
            SDC_TOPOLOGY_CLONE = 0x00000002,
            SDC_TOPOLOGY_EXTEND = 0x00000004,
            SDC_TOPOLOGY_EXTERNAL = 0x00000008,
            SDC_APPLY = 0x00000080,
            SDC_NO_OPTIMIZATION = 0x00000100,
            SDC_SAVE_TO_DATABASE = 0x00000200,
            SDC_ALLOW_CHANGES = 0x00000400,
            SDC_PATH_PERSIST_IF_REQUIRED = 0x00000800,
            SDC_FORCE_MODE_ENUMERATION = 0x00001000,
            SDC_ALLOW_PATH_ORDER_CHANGES = 0x00002000,
        }

        public enum DISPLAYCONFIG_TOPOLOGY_ID : uint
        {
            DISPLAYCONFIG_TOPOLOGY_INTERNAL = 0x00000001,
            DISPLAYCONFIG_TOPOLOGY_CLONE = 0x00000002,
            DISPLAYCONFIG_TOPOLOGY_EXTEND = 0x00000004,
            DISPLAYCONFIG_TOPOLOGY_EXTERNAL = 0x00000008,
        }
        [Flags]
        public enum QueryDisplayFlags : uint
        {
            QDC_ALL_PATHS = 0x00000001,
            QDC_ONLY_ACTIVE_PATHS = 0x00000002,
            QDC_DATABASE_CURRENT = 0x00000004,
            QDC_VIRTUAL_MODE_AWARE = 0x00000010,
            QDC_INCLUDE_HMD = 0x00000020,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_INFO
        {
            public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
            public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            public LUID adapterId;
            public uint id;
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
            public DISPLAYCONFIG_ROTATION rotation;
            public DISPLAYCONFIG_SCALING scaling;
            public DISPLAYCONFIG_RATIONAL refreshRate;
            public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
            public bool targetAvailable;
            public uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_TARGET_INFO
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
            public DISPLAYCONFIG_ROTATION rotation;
            public DISPLAYCONFIG_SCALING scaling;
            public DISPLAYCONFIG_RATIONAL refreshRate;
            public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
            public bool targetAvailable;
            public uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_MODE_INFO
        {
            public DISPLAYCONFIG_MODE_INFO_TYPE infoType;
            public uint id;
            public LUID adapterId;
            public DISPLAYCONFIG_TARGET_MODE targetMode;
            public DISPLAYCONFIG_SOURCE_MODE sourceMode;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_TARGET_MODE
        {
            public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_SOURCE_MODE
        {
            public uint width;
            public uint height;
            public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
            public POINTL position;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : int
        {
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = -1,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INDIRECT_WIRED = 16,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INDIRECT_VIRTUAL = 17,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_USB_C = 18,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_NETWORK = 19,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = unchecked((int)0x80000000),
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = unchecked((int)0xFFFFFFFF)
        }

        public enum DISPLAYCONFIG_ROTATION : uint
        {
            DISPLAYCONFIG_ROTATION_IDENTITY = 1,
            DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
            DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
            DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
            DISPLAYCONFIG_ROTATION_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_SCALING : uint
        {
            DISPLAYCONFIG_SCALING_IDENTITY = 1,
            DISPLAYCONFIG_SCALING_CENTERED = 2,
            DISPLAYCONFIG_SCALING_STRETCHED = 3,
            DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
            DISPLAYCONFIG_SCALING_CUSTOM = 5,
            DISPLAYCONFIG_SCALING_PREFERRED = 128,
            DISPLAYCONFIG_SCALING_FORCE_UINT32 = 0xFFFFFFFF
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_RATIONAL
        {
            public uint Numerator;
            public uint Denominator;
        }

        public enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
        {
            DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
            DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
            DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
        {
            DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
            DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
            DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_PIXELFORMAT : uint
        {
            DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
            DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
            DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
            DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
            DISPLAYCONFIG_PIXELFORMAT_NONGDI = 0x8000000,
            DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = 0xFFFFFFFF
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
        {
            public DISPLAYCONFIG_RATIONAL pixelRate;
            public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
            public uint videoStandard;
            public DISPLAYCONFIG_RATIONAL vSyncFreq;
            public DISPLAYCONFIG_RATIONAL hSyncFreq;
            public bool activeSizeAspectRatioValid;
            public uint activeSizeWidth;
            public uint activeSizeHeight;
            public uint totalSizeWidth;
            public uint totalSizeHeight;
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY videoOutputTechnology;
            public uint connectorInstanceId;
        }

        [Flags()]
        public enum ChangeDisplaySettingsFlags : uint
        {
            CDS_NONE = 0,
            CDS_UPDATEREGISTRY = 0x00000001,
            CDS_TEST = 0x00000002,
            CDS_FULLSCREEN = 0x00000004,
            CDS_GLOBAL = 0x00000008,
            CDS_SET_PRIMARY = 0x00000010,
            CDS_VIDEOPARAMETERS = 0x00000020,
            CDS_ENABLE_UNSAFE_MODES = 0x00000100,
            CDS_DISABLE_UNSAFE_MODES = 0x00000200,
            CDS_RESET = 0x40000000,
            CDS_RESET_EX = 0x20000000,
            CDS_NORESET = 0x10000000
        }

        public enum DISP_CHANGE : int
        {
            Successful = 0,
            Restart = 1,
            Failed = -1,
            BadMode = -2,
            NotUpdated = -3,
            BadFlags = -4,
            BadParam = -5,
            BadDualView = -6
        }

        class User_32
        {
            [DllImport("user32.dll")]
            public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

            [DllImport("user32.dll")]
            public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

            [DllImport("user32.dll")]
            public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

            [DllImport("user32.dll")]
            public static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, IntPtr lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern int SetDisplayConfig(uint numPathArrayElements, IntPtr pathArray, uint numModeArrayElements, IntPtr modeArray, SdcFlags flags);

            [DllImport("user32.dll")]
            public static extern int QueryDisplayConfig(QueryDisplayFlags flags, ref uint numPathArrayElements, [Out] DISPLAYCONFIG_PATH_INFO[] pathArray, ref uint numModeArrayElements, [Out] DISPLAYCONFIG_MODE_INFO[] modeArray, out DISPLAYCONFIG_TOPOLOGY_ID currentTopologyId);

            [DllImport("user32.dll")]
            public static extern int GetDisplayConfigBufferSizes(QueryDisplayFlags flags, out uint numPathArrayElements, out uint numModeArrayElements);
            public const int ENUM_CURRENT_SETTINGS = -1;
            public const int ENUM_REGISTRY_SETTINGS = -2;
            public const int DISP_CHANGE_SUCCESSFUL = 0;
            public const int DISP_CHANGE_RESTART = 1;
            public const int DISP_CHANGE_FAILED = -1;
            public const uint EDD_GET_DEVICE_INTERFACE_NAME = 0x00000001;
        }

        public class Displays
        {
            public static IList<string> GetDisplayNames()
            {
                var returnVals = new List<string>();
                for (var x = 0U; x < 1024; ++x)
                {
                    DISPLAY_DEVICE outVar = new DISPLAY_DEVICE();
                    outVar.cb = (ushort)Marshal.SizeOf(outVar);
                    if (User_32.EnumDisplayDevices(null, x, ref outVar, 1U))
                    {
                        returnVals.Add(outVar.DeviceName);
                    }
                }
                return returnVals;
            }

            public static IList<string> GetDesktopAttachedDisplayNames()
            {
                var returnVals = new List<string>();
                for (uint x = 0; x < 1024; ++x)
                {
                    DISPLAY_DEVICE outVar = new DISPLAY_DEVICE();
                    outVar.cb = (ushort)Marshal.SizeOf(outVar);
                    if (User_32.EnumDisplayDevices(null, x, ref outVar, 1U))
                    {
                        if ((outVar.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) == DisplayDeviceStateFlags.AttachedToDesktop)
                        {
                            returnVals.Add(outVar.DeviceName);
                        }
                    }
                }
                return returnVals;
            }

            public static IList<string> GetDisplayIds()
            {
                var returnVals = new List<string>();
                for (var x = 0U; x < 1024; ++x)
                {
                    DISPLAY_DEVICE outVar = new DISPLAY_DEVICE();
                    outVar.cb = (ushort)Marshal.SizeOf(outVar);
                    if (User_32.EnumDisplayDevices(null, x, ref outVar, 1U))
                    {
                        string deviceName = outVar.DeviceName;
                        if (User_32.EnumDisplayDevices(deviceName, 0, ref outVar, User_32.EDD_GET_DEVICE_INTERFACE_NAME))
                        {
                            returnVals.Add(outVar.DeviceID);
                        }
                    }
                }
                return returnVals;
            }

            public static Dictionary<string, string> GetDisplayDeviceNameToDisplayIdMapping()
            {
                var returnVals = new Dictionary<string, string>();
                for (var x = 0U; x < 1024; ++x)
                {
                    DISPLAY_DEVICE outVar = new DISPLAY_DEVICE();
                    outVar.cb = (ushort)Marshal.SizeOf(outVar);
                    if (User_32.EnumDisplayDevices(null, x, ref outVar, 1U))
                    {
                        string deviceName = outVar.DeviceName;
                        if (User_32.EnumDisplayDevices(deviceName, 0, ref outVar, User_32.EDD_GET_DEVICE_INTERFACE_NAME))
                        {
                            returnVals.Add(deviceName, outVar.DeviceID);
                        }
                    }
                }
                return returnVals;
            }

            public static Dictionary<string, string> GetDisplayDeviceIdToDisplayDeviceNameMapping()
            {
                var returnVals = new Dictionary<string, string>();
                for (var x = 0U; x < 1024; ++x)
                {
                    DISPLAY_DEVICE outVar = new DISPLAY_DEVICE();
                    outVar.cb = (ushort)Marshal.SizeOf(outVar);
                    if (User_32.EnumDisplayDevices(null, x, ref outVar, 1U))
                    {
                        string deviceName = outVar.DeviceName;
                        if (User_32.EnumDisplayDevices(deviceName, 0, ref outVar, User_32.EDD_GET_DEVICE_INTERFACE_NAME))
                        {
                            returnVals.Add(outVar.DeviceID, deviceName);
                        }
                    }
                }
                return returnVals;
            }

            public static IList<string> GetDisplayKeys()
            {
                var returnVals = new List<string>();
                for (var x = 0U; x < 1024; ++x)
                {
                    DISPLAY_DEVICE outVar = new DISPLAY_DEVICE();
                    outVar.cb = (ushort)Marshal.SizeOf(outVar);
                    if (User_32.EnumDisplayDevices(null, x, ref outVar, 1U))
                    {
                        returnVals.Add(outVar.DeviceKey);
                    }
                }
                return returnVals;
            }

            public static IList<string> GetDisplayStrings()
            {
                var returnVals = new List<string>();
                for (var x = 0U; x < 1024; ++x)
                {
                    DISPLAY_DEVICE outVar = new DISPLAY_DEVICE();
                    outVar.cb = (ushort)Marshal.SizeOf(outVar);
                    if (User_32.EnumDisplayDevices(null, x, ref outVar, 1U))
                    {
                        returnVals.Add(outVar.DeviceString);
                    }
                }
                return returnVals;
            }

            public static string GetCurrentResolution(string deviceName)
            {
                string returnValue = null;
                DEVMODE dm = GetDevMode1();
                if (0 != User_32.EnumDisplaySettings(deviceName, User_32.ENUM_CURRENT_SETTINGS, ref dm))
                {
                    returnValue = dm.dmPelsWidth + "," + dm.dmPelsHeight;
                }
                return returnValue;
            }

            public static string GetCurrentResolutionAndPosition(string deviceName)
            {
                string returnValue = null;
                DEVMODE dm = GetDevMode1();
                if (0 != User_32.EnumDisplaySettings(deviceName, User_32.ENUM_CURRENT_SETTINGS, ref dm))
                {
                    returnValue = dm.dmPelsWidth + "x" + dm.dmPelsHeight + " at " + dm.dmPosition.x + "," + dm.dmPosition.y;
                }
                return returnValue;
            }

            public static uint GetCurrentResolutionWidth(string deviceName)
            {
                DEVMODE dm = GetDevMode1();
                if (0 != User_32.EnumDisplaySettings(deviceName, User_32.ENUM_CURRENT_SETTINGS, ref dm))
                {
                    return dm.dmPelsWidth;
                }
                throw new InvalidOperationException("An error occurred getting current resolution width for '" + deviceName + "'.");
            }

            public static Dictionary<string, string> GetResolutions()
            {
                var displays = GetDisplayNames();
                var returnValue = new Dictionary<string, string>();
                foreach (var display in displays)
                {
                    returnValue.Add(display, GetCurrentResolution(display));
                }
                return returnValue;
            }

            public static Dictionary<string, string> GetResolutionsAndPositions()
            {
                var displays = GetDisplayNames();
                var returnValue = new Dictionary<string, string>();
                foreach (var display in displays)
                {
                    returnValue.Add(display, GetCurrentResolutionAndPosition(display));
                }
                return returnValue;
            }

            // public static void SetPrimaryDisplay(string deviceName)
            // {
            //     IntPtr nullPtr = IntPtr.Zero;
            //     DEVMODE primaryDevmode = new DEVMODE { dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODE)), dmFields = 0x20 };
            //     int result = User_32.ChangeDisplaySettingsEx(deviceName, ref primaryDevmode, nullPtr, ChangeDisplaySettingsFlags .CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags .CDS_SET_PRIMARY, nullPtr);		
            //     switch (result)
            //     {
            //         case User_32.DISP_CHANGE_SUCCESSFUL:
            //             Console.WriteLine("Primary display changed successfully to '" + deviceName + "'.");
            //             Console.WriteLine("Sleeping for 5 seconds...");
            //             Thread.Sleep(5000);
            //             Console.WriteLine("5 seconds have passed.");
            //             break;
            //         case User_32.DISP_CHANGE_RESTART:
            //             Console.WriteLine("You need to restart your computer for the changes to take effect.");
            //             break;
            //         default:
            //             Console.WriteLine("Failed to set the primary display. Error code: {result}");
            //             break;
            //     }
            // }

            public static uint ComputeDisplayModeScore(DEVMODE mode)
            {
                return mode.dmPelsWidth * mode.dmPelsHeight * mode.dmBitsPerPel + mode.dmDisplayFrequency;
            }

            /**
             * Compares two display modes to determine the best. 
             * Returns a negative integer, zero, or a positive integer 
             * as the first mode is worse than, equal to, or better than the 
             * second mode based on the "score" of the display mode
             **/
            public static int CompareDisplayModes(DEVMODE first, DEVMODE second)
            {
                uint firstRank = ComputeDisplayModeScore(first);
                uint secondRank = ComputeDisplayModeScore(second);
                if (firstRank < secondRank)
                {
                    return -1;
                }
                else if (firstRank > secondRank)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            public static DEVMODE GetOptimalDisplayMode(string deviceName)
            {
                DEVMODE mode = new DEVMODE();
                mode.dmSize = (ushort)Marshal.SizeOf(mode);

                int modeIndex = 0;
                int bestModeIndex = 0;
                uint bestScore = 0;

                //Console.WriteLine("Get optimal displaymode " + deviceName);

                User_32.EnumDisplaySettings(deviceName, modeIndex, ref mode);
                modeIndex++;

                while (0 != User_32.EnumDisplaySettings(deviceName, modeIndex, ref mode))
                {
                    uint currentModeRank = ComputeDisplayModeScore(mode);

                    //Console.WriteLine("Found a mode for " + deviceName + ": " + mode.dmPelsWidth + "x" + mode.dmPelsHeight + " at " + mode.dmDisplayFrequency + "Hz");

                    if (currentModeRank > bestScore)
                    {
                        bestModeIndex = modeIndex;
                        bestScore = currentModeRank;
                        System.Diagnostics.Debug.WriteLine("Found a better mode for " + deviceName + ": " + mode.dmPelsWidth + "x" + mode.dmPelsHeight + " at " + mode.dmDisplayFrequency + "Hz" + " at index " + modeIndex);
                    }
                    modeIndex++;
                }

                if (bestModeIndex < 0)
                {
                    throw new InvalidOperationException("An error occurred getting optimal display mode for '" + deviceName + "'.");
                }

                if (0 != User_32.EnumDisplaySettings(deviceName, bestModeIndex, ref mode))
                {
                    return mode;
                }

                throw new InvalidOperationException("An error occurred getting optimal display mode for '" + deviceName + "' at index " + bestModeIndex + ".");
            }

            public static DEVMODE GetCurrentDisplayMode(string deviceName)
            {
                DEVMODE dm = GetDevMode1();
                if (0 != User_32.EnumDisplaySettings(deviceName, User_32.ENUM_CURRENT_SETTINGS, ref dm))
                {
                    return dm;
                }
                throw new InvalidOperationException("An error occurred getting current display mode for '" + deviceName + "'.");
            }

            public static void SwitchToExtendModeIfClone()
            {
                uint pathArraySize = 0;
                uint modeArraySize = 0;
                DISPLAYCONFIG_TOPOLOGY_ID currentTopologyId;

                int bufferSizeResult = User_32.GetDisplayConfigBufferSizes(QueryDisplayFlags.QDC_ALL_PATHS, out pathArraySize, out modeArraySize);

                if (bufferSizeResult == 0)
                {
                    DISPLAYCONFIG_PATH_INFO[] pathArray = new DISPLAYCONFIG_PATH_INFO[pathArraySize];
                    DISPLAYCONFIG_MODE_INFO[] modeArray = new DISPLAYCONFIG_MODE_INFO[modeArraySize];

                    int queryResult = User_32.QueryDisplayConfig(QueryDisplayFlags.QDC_DATABASE_CURRENT, ref pathArraySize, pathArray, ref modeArraySize, modeArray, out currentTopologyId);

                    if (queryResult == 0)
                    {
                        if (currentTopologyId != DISPLAYCONFIG_TOPOLOGY_ID.DISPLAYCONFIG_TOPOLOGY_EXTEND)
                        {
                            User_32.SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, SdcFlags.SDC_APPLY | SdcFlags.SDC_TOPOLOGY_EXTEND);
                        }
                    }
                    else
                    {
                        Console.WriteLine("QueryDisplayConfig failed with error code: " + queryResult);
                    }
                }
                else
                {
                    Console.WriteLine("GetDisplayConfigBufferSizes failed with error code: " + bufferSizeResult);
                }
            }

            public static void Arrange(string primaryDisplayName, string[] leftDisplayNames, string[] rightDisplayNames)
            {
                const uint DM_PELSWIDTH = 0x00080000;
                const uint DM_PELSHEIGHT = 0x00100000;
                //const uint DM_BITSPERPEL = 0x00040000;
                const uint DM_POSITION = 0x00000020;
                //const uint DM_DISPLAYFREQUENCY = 0x00400000;
                //const uint DM_DISPLAYFLAGS = 0x00200000;

                DEVMODE optimalPrimaryDisplayMode = GetOptimalDisplayMode(primaryDisplayName);
                DEVMODE primaryDevMode = GetCurrentDisplayMode(primaryDisplayName);
                
                if (CompareDisplayModes(optimalPrimaryDisplayMode, primaryDevMode) > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Set primary " + primaryDisplayName + " to " + optimalPrimaryDisplayMode.dmPelsWidth + "x" + optimalPrimaryDisplayMode.dmPelsHeight + " at " + optimalPrimaryDisplayMode.dmDisplayFrequency + "Hz");
                    primaryDevMode = optimalPrimaryDisplayMode;
                    primaryDevMode.dmFields = DM_PELSWIDTH | DM_PELSHEIGHT; // | DM_BITSPERPEL | DM_DISPLAYFREQUENCY; // Update dmFields
                }

                primaryDevMode.dmFields |= DM_POSITION;
                primaryDevMode.dmPosition.x = 0;
                primaryDevMode.dmPosition.y = 0;

                Console.WriteLine("Set primary " + primaryDisplayName + " to " + primaryDevMode.dmPosition.x + "," + primaryDevMode.dmPosition.y);
                User_32.ChangeDisplaySettingsEx(primaryDisplayName, ref primaryDevMode, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_SET_PRIMARY | ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET, IntPtr.Zero);

                IntPtr nullPtr = IntPtr.Zero;
                int positionX = 0;
                int positionY = 0;

                Array.Reverse(leftDisplayNames);
                foreach (string displayName in leftDisplayNames)
                {
                    DEVMODE optimalDisplayMode = GetOptimalDisplayMode(displayName);
                    DEVMODE devMode = GetCurrentDisplayMode(displayName);
                    if (CompareDisplayModes(optimalDisplayMode, devMode) > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Set " + displayName + " to " + optimalDisplayMode.dmPelsWidth + "x" + optimalDisplayMode.dmPelsHeight + " at " + optimalDisplayMode.dmDisplayFrequency + "Hz");
                        devMode = optimalDisplayMode;
                        devMode.dmFields = DM_PELSWIDTH | DM_PELSHEIGHT; // | DM_BITSPERPEL | DM_DISPLAYFREQUENCY; // Update dmFields
                    }

                    positionX -= (int)GetCurrentResolutionWidth(displayName);
                    devMode.dmFields |= DM_POSITION;
                    devMode.dmPosition.x = positionX;
                    devMode.dmPosition.y = positionY;

                    Console.WriteLine("Set left " + displayName + " to " + devMode.dmPosition.x + "," + devMode.dmPosition.y);

                    User_32.ChangeDisplaySettingsEx(
                        displayName,
                        ref devMode,
                        (IntPtr)null,
                        ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET,
                        IntPtr.Zero);
                }

                positionX = (int)GetCurrentResolutionWidth(primaryDisplayName);

                foreach (string displayName in rightDisplayNames)
                {
                    DEVMODE optimalDisplayMode = GetOptimalDisplayMode(displayName);
                    DEVMODE devMode = GetCurrentDisplayMode(displayName);
                    if (CompareDisplayModes(optimalDisplayMode, devMode) > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Set " + displayName + " to " + optimalDisplayMode.dmPelsWidth + "x" + optimalDisplayMode.dmPelsHeight + " at " + optimalDisplayMode.dmDisplayFrequency + "Hz");
                        devMode = optimalDisplayMode;
                        devMode.dmFields = DM_PELSWIDTH | DM_PELSHEIGHT; // | DM_BITSPERPEL | DM_DISPLAYFREQUENCY; // Update dmFields
                    }

                    devMode.dmFields |= DM_POSITION;
                    devMode.dmPosition.x = positionX;
                    devMode.dmPosition.y = positionY;
                    Console.WriteLine("Set right " + displayName + " to " + positionX + "," + positionY);

                    User_32.ChangeDisplaySettingsEx(
                        displayName,
                        ref devMode,
                        (IntPtr)null,
                        ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET,
                        IntPtr.Zero);

                    positionX += (int)GetCurrentResolutionWidth(displayName);
                }

                // Apply the settings and update the registry
                User_32.ChangeDisplaySettingsEx(null, IntPtr.Zero, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_NONE, (IntPtr)null);
            }

            private static DEVMODE GetDevMode1()
            {
                DEVMODE dm = new DEVMODE();
                dm.dmDeviceName = new string(new char[32]);
                dm.dmFormName = new string(new char[32]);
                dm.dmSize = (ushort)Marshal.SizeOf(dm);
                return dm;
            }
        }
    }
}