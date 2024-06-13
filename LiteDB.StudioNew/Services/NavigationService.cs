using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LiteDB.StudioNew.ViewModels;
using LiteDB.StudioNew.Views;

namespace LiteDB.StudioNew.Services;

public class NavigationService : INavigationService
{
    public void NavigateToConnectionsListViewModel()
    {
        var vm = new ConnectionListViewModel(Container.ConnectionRepository, this);
        var view = new ConnectionListView
        {
            DataContext = vm
        };

        view.ShowDialog(GetMainWidow());
    }

    public void NavigateToAddConnectionViewModel()
    {
        var vm = new AddConnectionViewModel(Container.ConnectionRepository);
        var view = new AddConnectionView
        {
            DataContext = vm
        };

        view.ShowDialog(GetMainWidow());
    }

    public void NavigateToEditConnectionViewModel(Guid connectionGuid)
    {
        var vm = new EditConnectionViewModel(connectionGuid, Container.ConnectionRepository);
        var view = new EditConnectionView
        {
            DataContext = vm
        };

        view.ShowDialog(GetMainWidow());
    }

    private static Window GetMainWidow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) 
            return desktop.MainWindow;

        throw new InvalidOperationException();
    }
}