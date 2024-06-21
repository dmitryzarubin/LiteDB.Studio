using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LiteDB.StudioNew.ViewModels;
using LiteDB.StudioNew.Views;

namespace LiteDB.StudioNew.Services;

public class NavigationService : INavigationService
{
    private readonly Stack<(object viewMode, Window view, TaskCompletionSource<object> windowClosed)> _stack = new();


    public Task<object> NavigateToConnectionsListViewModel()
    {
        var vm = new ConnectionListViewModel(Container.ConnectionRepository, this);
        var view = new ConnectionListView
        {
            DataContext = vm
        };

        var parentWindow = _stack.TryPeek(out var tuple) ? tuple.view : GetMainWidow();
        var windowClosed = new TaskCompletionSource<object>();
        _stack.Push((vm, view, windowClosed));
        view.Closed += ViewOnClosed;

        view.ShowDialog(parentWindow);
        return windowClosed.Task;
    }

    public void NavigateToAddConnectionViewModel()
    {
        var vm = new AddConnectionViewModel(Container.ConnectionRepository, Container.NavigationService);
        var view = new AddConnectionView
        {
            DataContext = vm
        };

        var parentWindow = _stack.TryPeek(out var tuple) ? tuple.view : GetMainWidow();
        _stack.Push((vm, view, new TaskCompletionSource<object>()));
        view.Closed += ViewOnClosed;
        
        view.ShowDialog(parentWindow);
    }

    public void NavigateToEditConnectionViewModel(Guid connectionGuid)
    {
        var vm = new EditConnectionViewModel(connectionGuid, Container.ConnectionRepository, Container.NavigationService);
        var view = new EditConnectionView
        {
            DataContext = vm
        };

        var parentWindow = _stack.TryPeek(out var tuple) ? tuple.view : GetMainWidow();
        _stack.Push((vm, view, new TaskCompletionSource<object>()));
        view.Closed += ViewOnClosed;
        
        view.ShowDialog(parentWindow);
    }

    public void Close()
    {
        var (viewMode, view, windowClosed) = _stack.Pop();
        windowClosed.TrySetResult(viewMode);
        view.Closed -= ViewOnClosed;
        
        view.Close();
    }

    private void ViewOnClosed(object? sender, EventArgs e)
    {
        var (viewMode, view, windowClosed) = _stack.Pop();
        windowClosed.TrySetResult(viewMode);
        view.Closed -= ViewOnClosed;
    }

    private static Window GetMainWidow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;

        throw new InvalidOperationException();
    }
}