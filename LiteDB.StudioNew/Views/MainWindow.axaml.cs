using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LiteDB.StudioNew.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Container.NavigationService.NavigateToConnectionsListViewModel();
    }
}