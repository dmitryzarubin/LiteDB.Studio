using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using DynamicData;
using LiteDB.StudioNew.Models;
using LiteDB.StudioNew.Services;
using ReactiveUI;

namespace LiteDB.StudioNew.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public MainWindowViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        ConnectCommand = ReactiveCommand.CreateFromTask(() =>
        {
            return ConnectAsync();
        }, outputScheduler: RxApp.TaskpoolScheduler);
        ConnectCommand.ThrownExceptions.Subscribe(ex =>
        {
            Error.Handle(ex.Message);
            Debug.WriteLine($"{DateTime.Now:s} - {ex}");
        });
    }
    
    public ObservableCollection<DataBaseItemViewModel> Databases { get; } = new();
    public ObservableCollection<QueryViewModel> Queries { get; } = new();
    
    public Interaction<string, Unit> Error { get; } = new();
    
    public ReactiveCommand<Unit, Unit> ConnectCommand { get; set; }

    private DataBaseItemViewModel CreateDatabaseHierarchy(Database database)
    {
        var collectionViewModels = new ObservableCollection<DataBaseItemViewModel>([CreateSystemCollections()]);
        collectionViewModels.AddRange(CreateUserCollections());
        
        return new DataBaseItemViewModel(database.ConnectionName, DataBaseItemsType.Database, database,
            new List<(string title, Action action)>([
                ("Disconnect", () => Disconnect(database))
            ]), collectionViewModels);

        DataBaseItemViewModel CreateSystemCollections()
        {
            return new DataBaseItemViewModel("System", DataBaseItemsType.SystemCollectionFolder, null!,
                new List<(string title, Action action)>(),
                new ObservableCollection<DataBaseItemViewModel>(
                    database.Collections.Where(c => c.IsSystem).Select(CreateSystemCollection).OrderBy(c => c.Title)
                        .ToArray()
                ));
        }

        DataBaseItemViewModel CreateSystemCollection(Collection collection)
        {
            return new DataBaseItemViewModel(collection.Name, DataBaseItemsType.SystemCollection, collection,
                new List<(string title, Action action)>([
                    ("Select all", () =>
                    {
                        var queryViewModel = new QueryViewModel(collection, "SELECT * FROM "+ collection.Name);
                        Queries.Add(queryViewModel);
                    })
                ]));
        }

        IEnumerable<DataBaseItemViewModel> CreateUserCollections()
        {
            return database.Collections.Where(c => !c.IsSystem).Select(c =>
            {
                return new DataBaseItemViewModel(c.Name, DataBaseItemsType.Collection, c,
                    new List<(string title, Action action)>([
                    ("Test", () => { })]),
                    new ObservableCollection<DataBaseItemViewModel>([CreateIndexes(c)]));
            });
        }

        DataBaseItemViewModel CreateIndexes(Collection collection)
        {
            return new DataBaseItemViewModel("Indices", DataBaseItemsType.IndexFolder, null,
                new List<(string title, Action action)>(),
                new ObservableCollection<DataBaseItemViewModel>(
                    collection.Indices.Select(i => new DataBaseItemViewModel(i.Name, DataBaseItemsType.Index, i,
                            new List<(string title, Action action)>()))
                        .OrderBy(c => c.Title)
                        .ToArray()
                ));
        }
    }

    private void QueryAllDocumentsInCollection(Collection collection)
    {
        var queryViewModel = new QueryViewModel(collection);
    }


    private async Task ConnectAsync()
    {
        var result = await _navigationService.NavigateToConnectionsListViewModel(true);
        var connectionListViewModel = (ConnectionListViewModel)result;
        if (!connectionListViewModel.IsSelected || connectionListViewModel.SelectedConnection == null)
            return;

        var database = new Database(connectionListViewModel.SelectedConnection);
        database.Connect();
        
        var dataBaseViewModel = CreateDatabaseHierarchy(database);
        
        RxApp.MainThreadScheduler.Schedule(string.Empty,(sc,s ) =>
        {
            Databases.Add(dataBaseViewModel);
            return Disposable.Empty;
        });
    }

    private void Disconnect(Database database)
    {
        database.Disconnect();
        var toRemove = Databases
            .Where(x => x.ItemsType == DataBaseItemsType.Database)
            .Where(x => x.RelatedObject == database)
            .ToArray();

        Databases.Remove(toRemove);
    }
    
}

public enum DataBaseItemsType
{
    Database,
    SystemCollectionFolder,
    SystemCollection,
    Collection,
    IndexFolder,
    Index
}