using System;

namespace TimeTrace.Models;

public class TimeEntry
{
    public DateTime Timestamp { get; set; }
    public string Mode { get; set; } = "";
    public string Tag { get; set; } = "";
    public int SessionSeconds { get; set; }
    public int CumulativeSeconds { get; set; }
    public string SessionTime => $"{SessionSeconds / 3600:D2}:{(SessionSeconds % 3600) / 60:D2}:{SessionSeconds % 60:D2}";
    public string CumulativeTime => $"{CumulativeSeconds / 3600:D2}:{(CumulativeSeconds % 3600) / 60:D2}:{CumulativeSeconds % 60:D2}";
}
