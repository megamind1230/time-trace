using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TimeTrace.Models;

namespace TimeTrace.Services;

public static class TimeService
{
    private static readonly string Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "magnus", "timetrace");
    private static readonly string FileName = Path.Combine(Dir, "timetrace.log");

    public static List<TimeEntry> LoadEntries()
    {
        var entries = new List<TimeEntry>();
        if (!File.Exists(FileName)) return entries;

        foreach (var line in File.ReadAllLines(FileName))
        {
            if (string.IsNullOrWhiteSpace(line) || !line.TrimStart().StartsWith('|'))
                continue;

            var parts = line.Split('|', StringSplitOptions.TrimEntries);
            if (parts.Length < 6) continue;
            if (!DateTime.TryParse(parts[1], out var ts)) continue;

            entries.Add(new TimeEntry
            {
                Timestamp = ts,
                Mode = parts[2],
                Tag = parts[3],
                SessionSeconds = ParseDuration(parts[4]),
                CumulativeSeconds = ParseDuration(parts[5])
            });
        }

        return entries;
    }

    public static void SaveEntry(string tag, int minutes, int seconds, string mode)
    {
        Directory.CreateDirectory(Dir);

        var entries = File.Exists(FileName) ? LoadEntries() : new List<TimeEntry>();
        var currentSec = minutes * 60 + seconds;

        entries.Add(new TimeEntry
        {
            Timestamp = DateTime.Now,
            Mode = mode,
            Tag = tag,
            SessionSeconds = currentSec,
            CumulativeSeconds = 0 // recalculated in WriteAll
        });

        WriteAll(entries);
    }

    public static void ResetAll()
    {
        if (File.Exists(FileName)) File.Delete(FileName);
    }

    public static string ReadRaw()
    {
        return File.Exists(FileName) ? File.ReadAllText(FileName) : "";
    }

    private static void WriteAll(List<TimeEntry> entries)
    {
        var ordered = entries.OrderBy(e => e.Timestamp).ToList();
        var cumMap = new Dictionary<string, int>();
        foreach (var e in ordered)
        {
            cumMap.TryGetValue(e.Tag, out var prev);
            e.CumulativeSeconds = prev + e.SessionSeconds;
            cumMap[e.Tag] = e.CumulativeSeconds;
        }

        var lines = new List<string>
        {
            "| Timestamp            | Mode      | Task Name          | Session Time      | Cumulative Time   |",
            "|----------------------+-----------+--------------------+-------------------+-------------------|"
        };

        foreach (var e in ordered)
        {
            var ts = e.Timestamp.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            lines.Add($"| {ts,-20} | {e.Mode,-9} | {e.Tag,-18} | {e.SessionTime,-17} | {e.CumulativeTime,-17} |");
        }

        File.WriteAllLines(FileName, lines);
    }

    private static int ParseDuration(string s)
    {
        var parts = s.Split(':');
        if (parts.Length == 3 &&
            int.TryParse(parts[0], out var h) &&
            int.TryParse(parts[1], out var m) &&
            int.TryParse(parts[2], out var sec))
            return h * 3600 + m * 60 + sec;
        return 0;
    }
}
