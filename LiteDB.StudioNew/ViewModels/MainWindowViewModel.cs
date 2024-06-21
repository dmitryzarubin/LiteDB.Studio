using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using AvaloniaEdit.Utils;
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
    }


    public ObservableCollection<DataBaseItemViewModel> Databases { get; } = new();
    public ObservableCollection<QueryViewModel> Queries { get; } = new();

    private DataBaseItemViewModel CreateDatabaseHierarchy(Database database)
    {
        var collectionViewModels = new ObservableCollection<DataBaseItemViewModel>([CreateSystemCollections()]);
        collectionViewModels.AddRange(CreateUserCollections());


        return new DataBaseItemViewModel(database.ConnectionName, DataBaseItemsType.Database,
            new List<ReactiveCommand<Unit, Unit>>(), collectionViewModels);

        DataBaseItemViewModel CreateSystemCollections()
        {
            return new DataBaseItemViewModel("System", DataBaseItemsType.SystemCollectionFolder,
                new List<ReactiveCommand<Unit, Unit>>(),
                new ObservableCollection<DataBaseItemViewModel>(
                    database.Collections.Where(c => c.IsSystem).Select(CreateSystemCollection).OrderBy(c => c.Title)
                        .ToArray()
                ));
        }

        DataBaseItemViewModel CreateSystemCollection(Collection collection)
        {
            return new DataBaseItemViewModel(collection.Name, DataBaseItemsType.SystemCollection,
                new List<ReactiveCommand<Unit, Unit>>());
        }

        IEnumerable<DataBaseItemViewModel> CreateUserCollections()
        {
            return database.Collections.Where(c => !c.IsSystem).Select(c =>
            {
                return new DataBaseItemViewModel(c.Name, DataBaseItemsType.Collection,
                    new List<ReactiveCommand<Unit, Unit>>(
                    [
                        ReactiveCommand.Create(() => QueryAllDocumentsInCollection(c))
                    ]),
                    new ObservableCollection<DataBaseItemViewModel>([CreateIndexes(c)]));
            });
        }

        DataBaseItemViewModel CreateIndexes(Collection collection)
        {
            return new DataBaseItemViewModel("Indices", DataBaseItemsType.IndexFolder,
                new List<ReactiveCommand<Unit, Unit>>(),
                new ObservableCollection<DataBaseItemViewModel>(
                    collection.Indices.Select(i => new DataBaseItemViewModel(i.Name, DataBaseItemsType.Index,
                            new List<ReactiveCommand<Unit, Unit>>()))
                        .OrderBy(c => c.Title)
                        .ToArray()
                ));
        }
    }

    private void QueryAllDocumentsInCollection(Collection collection)
    {
        var queryViewModel = new QueryViewModel(collection);
    }


    private void AddDatabase()
    {
        var result = _navigationService.NavigateToConnectionsListViewModel().GetAwaiter().GetResult();
        var connectionListViewModel = (ConnectionListViewModel)result;
        
        if (connectionListViewModel.SelectedConnection == null)
            return;

        var database = new Database(connectionListViewModel.SelectedConnection);
        
        var dataBaseViewModel = CreateDatabaseHierarchy(database);
        Databases.Add(dataBaseViewModel);
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