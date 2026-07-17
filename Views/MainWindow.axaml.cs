using Avalonia.Controls;
using TimeTrace.ViewModels;

namespace TimeTrace.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}