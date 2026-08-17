using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BroDisplaySetup
{
    // Writes a running, plain-text diagnostic log to disk for a single app session, covering both
    // (a) the resolution the app *computes* as optimal per display (see
    // DisplayInfo.OptimalResolutionDiagnostics) and (b) what it actually *attempts to set* during
    // arrangement (see Extern.Displays.Arrange) - these can differ if the underlying Win32 calls
    // don't apply cleanly on a given display/driver, which is exactly the kind of real-hardware-only
    // behavior this log exists to catch without needing a debugger attached.
    //
    // Plain File I/O (not System.Diagnostics.Debug/Trace, which compiles to a no-op in Release
    // builds) so this works the same whether launched from Visual Studio (F5, Debug) or as the
    // published Release exe. Retrieved via "Öppna diagnostiklogg..." in the app's Advancerat menu,
    // or read directly from LogFilePath, so a real-hardware run can be captured and inspected
    // without needing a debugger attached mid-session.
    static class DiagnosticsLog
    {
        public static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BroDisplaySetup",
            "diagnostics.log");

        // Called once at app startup so each session's log starts clean instead of appending
        // indefinitely across every past run.
        public static void StartNewSession()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
                File.WriteAllText(LogFilePath, $"BroDisplaySetup diagnostics - session started {DateTime.Now:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}");
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // Best-effort diagnostics only - never let a logging failure break the real app.
            }
        }

        public static void LogResolutionSelection(IEnumerable<DisplayInfo> displayInfoList)
        {
            var lines = new StringBuilder();
            lines.AppendLine();
            lines.AppendLine($"--- Resolution selection ({DateTime.Now:HH:mm:ss}) ---");

            foreach (DisplayInfo displayInfo in displayInfoList)
            {
                lines.AppendLine($"{displayInfo.UserFriendlyName} ({displayInfo.DeviceName}):");
                lines.AppendLine($"  Computed optimal: {displayInfo.OptimalResolution.Width}x{displayInfo.OptimalResolution.Height}");
                lines.AppendLine($"  Reason: {displayInfo.OptimalResolutionDiagnostics}");
                lines.AppendLine($"  PhysicalSize: {displayInfo.PhysicalWidthCm} x {displayInfo.PhysicalHeightCm} cm ({displayInfo.DiagonalInches:0.#}\" diagonal)");
                lines.AppendLine($"  DpiScaling: {displayInfo.DpiScalingInfo.Current}% (recommended: {displayInfo.DpiScalingInfo.Recommended}%)");
            }

            AppendText(lines.ToString());
        }

        public static void AppendLine(string line = "")
        {
            AppendText(line + Environment.NewLine);
        }

        private static void AppendText(string text)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
                File.AppendAllText(LogFilePath, text);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // Best-effort diagnostics only - never let a logging failure break the real app.
            }
        }
    }
}
