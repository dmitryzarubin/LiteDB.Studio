using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using LiteDB.StudioNew.Models;
using LiteDB.StudioNew.Services;
using ReactiveUI;

namespace LiteDB.StudioNew.ViewModels;

public class ConnectionListViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly IConnectionRepository _connectionRepository;
    private readonly INavigationService _navigationService;
    private ReadOnlyObservableCollection<Connection> _connections = ReadOnlyObservableCollection<Connection>.Empty;

    public ConnectionListViewModel(IConnectionRepository connectionRepository, INavigationService navigationService)
    {
        _connectionRepository = connectionRepository;
        _navigationService = navigationService;

        this.WhenActivated(disposable =>
        {
            _connectionRepository.ConnectionsObservable
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _connections)
                .Subscribe()
                .DisposeWith(disposable);
        });
    }


    public ReadOnlyObservableCollection<Connection> Connections => _connections;

    public Connection? SelectedConnection { get; set; }

    public ViewModelActivator Activator { get; } = new();


    public void AddConnection()
    {
        _navigationService.NavigateToAddConnectionViewModel();
    }

    public void EditConnection()
    {
        if (SelectedConnection == null)
            return;

        _navigationService.NavigateToEditConnectionViewModel(SelectedConnection.Guid);
    }

    public void RemoveConnection()
    {
        if (SelectedConnection == null)
            return;

        // Confirm

        _connectionRepository.RemoveAsync(SelectedConnection.Guid);
    }

    public void SelectConnection()
    {
    }
}