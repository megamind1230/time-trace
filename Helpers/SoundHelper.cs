using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace TimeTrace.Helpers;

public static class SoundHelper
{
    public static void Play(string filePath)
    {
        if (!File.Exists(filePath)) return;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Run("powershell", $"-c (New-Object Media.SoundPlayer '{filePath}').PlaySync()");
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            Run("afplay", filePath);
        else
            Run(File.Exists("/usr/bin/paplay") ? "paplay" : "aplay", filePath);
    }

    private static void Run(string cmd, string args)
    {
        try { Process.Start(new ProcessStartInfo(cmd, args) { CreateNoWindow = true, UseShellExecute = false })?.Dispose(); }
        catch { /* player not available, move on */ }
    }
}
