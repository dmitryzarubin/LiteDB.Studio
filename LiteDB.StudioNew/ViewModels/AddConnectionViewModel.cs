using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using LiteDB.StudioNew.Models;
using LiteDB.StudioNew.Services;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace LiteDB.StudioNew.ViewModels;

public class AddConnectionViewModel : ViewModelBase
{
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

    public AddConnectionViewModel(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository ?? throw new ArgumentNullException(nameof(connectionRepository));

        CulturesList = StaticDictionaries.CulturesList;
        SortsList = StaticDictionaries.SortsList;

        this.ValidationRule(vm => vm.Name, string.IsNullOrWhiteSpace, "Name must be specified");
        this.ValidationRule(vm => vm.Path, string.IsNullOrWhiteSpace, "Path must be specified");

        AddConnectionCommand = ReactiveCommand.CreateFromTask(AddConnection, ValidationContext.Valid, RxApp.TaskpoolScheduler);
        AddConnectionCommand.ThrownExceptions.Subscribe(ex =>
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
    public ReactiveCommand<Unit, Unit> AddConnectionCommand { get; set; }
    
    private async Task AddConnection()
    {
        var connection = new Connection
        {
            Guid = Guid.NewGuid(),
            Name = Name ?? throw new ArgumentNullException(nameof(Name)),
            Password = Password ?? throw new ArgumentNullException(nameof(Password)),
            Path = Path ?? throw new ArgumentNullException(nameof(Path)),
            ConnectionType = ConnectionType,

            InitialSize = InitialSize,
            ReadOnly = ReadOnly,
            Upgrade = Upgrade,

            Culture = Culture,
            Sort = Sort
        };

        await _connectionRepository.InsertAsync(connection);
    }
}