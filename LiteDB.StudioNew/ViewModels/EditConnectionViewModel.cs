using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LiteDB.StudioNew.Services;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace LiteDB.StudioNew.ViewModels;

public class EditConnectionViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly Guid _connectionGuid;
    private readonly IConnectionRepository _connectionRepository;
    private ConnectionType _connectionType = ConnectionType.Direct;
    private string? _culture;
    private int _initialSize;
    private string? _name;
    private string? _password;
    private string? _path;
    private bool _readOnly;
    private string? _sort;
    private bool _upgrade;

    public EditConnectionViewModel(Guid connectionGuid, IConnectionRepository connectionRepository)
    {
        _connectionGuid = connectionGuid;
        _connectionRepository = connectionRepository ?? throw new ArgumentNullException(nameof(connectionRepository));
        
        this.WhenActivated(disposable =>
        {
            Observable.Start(() =>
            {
                var connection = _connectionRepository.Get(_connectionGuid);
                if (connection == null)
                   throw new InvalidOperationException();

                return connection;
            }, RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(c =>
            {
                Name = c.Name;
                Path = c.Path;
                Password = c.Password;
                ConnectionType = c.ConnectionType;
                ReadOnly = c.ReadOnly;
                Upgrade = c.Upgrade;
                InitialSize = c.InitialSize;
                Culture = c.Culture;
                Sort = c.Sort;
            })
            .DisposeWith(disposable);
        });
        
        CulturesList = StaticDictionaries.CulturesList;
        SortsList = StaticDictionaries.SortsList;

        this.ValidationRule(vm => vm.Name, string.IsNullOrWhiteSpace, "Name must be specified");
        this.ValidationRule(vm => vm.Path, string.IsNullOrWhiteSpace, "Path must be specified");

        UpdateConnectionCommand = ReactiveCommand.CreateFromTask(UpdateConnection, ValidationContext.Valid, RxApp.TaskpoolScheduler);
        UpdateConnectionCommand.ThrownExceptions.Subscribe(ex =>
        {
            Error.Handle(ex.Message);
            Debug.WriteLine($"{DateTime.Now:s} - {ex}");
        });
    }

    public string? Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string? Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public string? Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public ConnectionType ConnectionType
    {
        get => _connectionType;
        set => this.RaiseAndSetIfChanged(ref _connectionType, value);
    }

    public bool ReadOnly
    {
        get => _readOnly;
        set
        {
            this.RaiseAndSetIfChanged(ref _readOnly, value);
            if (value)
                this.RaiseAndSetIfChanged(ref _upgrade, false, nameof(Upgrade));
        }
    }

    public bool Upgrade
    {
        get => _upgrade;
        set => this.RaiseAndSetIfChanged(ref _upgrade, value);
    }

    public int InitialSize
    {
        get => _initialSize;
        set => this.RaiseAndSetIfChanged(ref _initialSize, value);
    }


    public IReadOnlyList<string> CulturesList { get; }

    public string? Culture
    {
        get => _culture;
        set => this.RaiseAndSetIfChanged(ref _culture, value);
    }


    public IReadOnlyList<string> SortsList { get; }

    public string? Sort
    {
        get => _sort;
        set => this.RaiseAndSetIfChanged(ref _sort, value);
    }


    public Interaction<string, Unit> Error { get; } = new();
    
    public ReactiveCommand<Unit, Unit> UpdateConnectionCommand { get; set; }


    public ViewModelActivator Activator { get; } = new();

    private async Task UpdateConnection()
    {
        var connection = _connectionRepository.Get(_connectionGuid);
        if (connection == null)
            throw new InvalidOperationException();

        connection.Name = Name ?? throw new ArgumentNullException(nameof(Name));
        connection.Password = Password ?? throw new ArgumentNullException(nameof(Password));
        connection.Path = Path ?? throw new ArgumentNullException(nameof(Path));
        connection.ConnectionType = ConnectionType;

        connection.InitialSize = InitialSize;
        connection.ReadOnly = ReadOnly;
        connection.Upgrade = Upgrade;

        connection.Culture = Culture;
        connection.Sort = Sort;

        await _connectionRepository.UpdateAsync(connection);
    }
}