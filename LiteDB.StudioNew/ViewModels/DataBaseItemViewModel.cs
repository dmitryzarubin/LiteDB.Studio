using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace LiteDB.StudioNew.ViewModels;

public class DataBaseItemViewModel : ReactiveObject
{
    public DataBaseItemViewModel(string title, DataBaseItemsType itemsType,
        IEnumerable<ReactiveCommand<Unit, Unit>> contextActions)
    {
        Title = title;
        ContextActions = contextActions;
        ItemsType = itemsType;
    }

    public DataBaseItemViewModel(string title, DataBaseItemsType itemsType,
        IEnumerable<ReactiveCommand<Unit, Unit>> contextActions, ObservableCollection<DataBaseItemViewModel> subNodes)
    {
        Title = title;
        SubNodes = subNodes;
        ItemsType = itemsType;
        ContextActions = contextActions;
    }

    public string Title { get; }
    public DataBaseItemsType ItemsType { get; }
    public ObservableCollection<DataBaseItemViewModel>? SubNodes { get; }
    public IEnumerable<ReactiveCommand<Unit, Unit>> ContextActions { get; }
}