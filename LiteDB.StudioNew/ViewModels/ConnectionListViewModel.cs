using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
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
    private Connection? _selectedConnection;

    public ConnectionListViewModel(bool isSelectionMode, IConnectionRepository connectionRepository, INavigationService navigationService)
    {
        IsSelectionMode = isSelectionMode;
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

        var canEditConnection = this.WhenAnyValue(x => x.SelectedConnection)
            .Select(c => c != null);

        AddConnectionCommand = ReactiveCommand.Create(AddConnection);
        AddConnectionCommand.ThrownExceptions.Subscribe(ex =>
        {
            Error.Handle(ex.Message);
            Debug.WriteLine($"{DateTime.Now:s} - {ex}");
        });

        EditConnectionCommand = ReactiveCommand.Create(EditConnection, canEditConnection);
        EditConnectionCommand.ThrownExceptions.Subscribe(ex =>
        {
            Error.Handle(ex.Message);
            Debug.WriteLine($"{DateTime.Now:s} - {ex}");
        });

        RemoveConnectionCommand = ReactiveCommand.Create(RemoveConnection, canEditConnection);
        RemoveConnectionCommand.ThrownExceptions.Subscribe(ex =>
        {
            Error.Handle(ex.Message);
            Debug.WriteLine($"{DateTime.Now:s} - {ex}");
        });
        
        SelectConnectionCommand = ReactiveCommand.Create(SelectConnection, canEditConnection);
        CloseCommand = ReactiveCommand.Create(CloseConnection);
    }

    public ReadOnlyObservableCollection<Connection> Connections => _connections;

    public bool IsSelectionMode { get; }
    public bool IsSelected { get; private set; }

    public Connection? SelectedConnection
    {
        get => _selectedConnection;
        set => this.RaiseAndSetIfChanged(ref _selectedConnection, value);
    }

    public Interaction<string, Unit> Error { get; } = new();
    public Interaction<Unit, bool> ConfirmDeletion { get; } = new();


    public ReactiveCommand<Unit, Unit> AddConnectionCommand { get; }
    public ReactiveCommand<Unit, Unit> EditConnectionCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveConnectionCommand { get; }

    public ReactiveCommand<Unit,Unit> SelectConnectionCommand { get; }
    public ReactiveCommand<Unit,Unit> CloseCommand { get; }


    public ViewModelActivator Activator { get; } = new();

    private void AddConnection()
    {
        _navigationService.NavigateToAddConnectionViewModel();
    }

    private void EditConnection()
    {
        if (SelectedConnection == null)
            return;

        _navigationService.NavigateToEditConnectionViewModel(SelectedConnection.Guid);
    }

    private void RemoveConnection()
    {
        if (SelectedConnection == null)
            return;

        ConfirmDeletion.Handle(Unit.Default).Subscribe(confirmed =>
        {
            if (confirmed)
                _connectionRepository.RemoveAsync(SelectedConnection.Guid);
        });
    }

    private void SelectConnection()
    {
        IsSelected = true;
        _navigationService.Close();
    }
    
    private void CloseConnection()
    {
        _navigationService.Close();
    }
}