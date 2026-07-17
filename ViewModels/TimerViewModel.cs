using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeTrace.Helpers;
using TimeTrace.Services;

namespace TimeTrace.ViewModels;

public partial class TimerViewModel : ObservableObject
{
    [ObservableProperty]
    private string _taskName = "";

    [ObservableProperty]
    private int _hours;

    [ObservableProperty]
    private int _minutes;

    [ObservableProperty]
    private int _seconds;

    [ObservableProperty]
    private string _remainingTime = "00:00:00";

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private bool _isFinished;

    private System.Timers.Timer? _timer;
    private DateTime _endTime;
    private int _origHours, _origMinutes, _origSeconds; //#baka snapshot of configured duration at start

    private static readonly string SoundPath = System.IO.Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "timer-sound.mp3");

    partial void OnTaskNameChanged(string value)
    {
        IsFinished = false;
    }

    [RelayCommand]
    private void StartTimer()
    {
        if (string.IsNullOrWhiteSpace(TaskName))
            return;

        var totalSeconds = Hours * 3600 + Minutes * 60 + Seconds;
        if (totalSeconds <= 0)
            return;

        IsRunning = true;
        IsFinished = false;
        _origHours = Hours; _origMinutes = Minutes; _origSeconds = Seconds;
        _endTime = DateTime.Now.AddSeconds(totalSeconds);

        _timer = new System.Timers.Timer(100);
        _timer.Elapsed += (s, e) => UpdateRemaining();
        _timer.Start();
    }

    private void UpdateRemaining()
    {
        var remaining = _endTime - DateTime.Now;
        if (remaining.TotalSeconds <= 0)
        {
            RemainingTime = "00:00:00";
            _timer?.Stop();
            IsRunning = false;
            IsFinished = true;

            var totalMin = _origHours * 60 + _origMinutes;
            TimeService.SaveEntry(TaskName, totalMin, _origSeconds, "timer");
            SoundHelper.Play(SoundPath);
        }
        else
        {
            RemainingTime = remaining.ToString(@"hh\:mm\:ss");
        }
    }

    [RelayCommand]
    private void StopTimer()
    {
        _timer?.Stop();
        IsRunning = false;

        var remaining = _endTime - DateTime.Now; //#baka this is remaining, not elapsed
        var totalSec = (int)Math.Max(0, (_origHours * 3600 + _origMinutes * 60 + _origSeconds) - remaining.TotalSeconds);
        var min = totalSec / 60;
        var sec = totalSec % 60;

        TimeService.SaveEntry(TaskName, min, sec, "timer");
    }

    [RelayCommand]
    private void Reset()
    {
        _timer?.Stop();
        IsRunning = false;
        IsFinished = false;
        RemainingTime = "00:00:00";
        Hours = 0;
        Minutes = 0;
        Seconds = 0;
    }
}
