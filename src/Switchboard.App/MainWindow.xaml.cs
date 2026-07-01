using System.Windows;
using Switchboard.App.ViewModels;

namespace Switchboard.App;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
