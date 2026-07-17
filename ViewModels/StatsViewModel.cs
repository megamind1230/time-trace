using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeTrace.Services;

namespace TimeTrace.ViewModels;

public partial class StatsViewModel : ObservableObject
{
    [ObservableProperty]
    private string _rawLog = "";

    public StatsViewModel()
    {
        LoadStats();
    }

    [RelayCommand]
    private void LoadStats()
    {
        RawLog = TimeService.ReadRaw();
    }

    [RelayCommand]
    private void ResetLog()
    {
        TimeService.ResetAll();
        LoadStats();
    }
}
