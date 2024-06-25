using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;

namespace LiteDB.StudioNew.ViewModels;

public class DataBaseItemViewModel : ReactiveObject
{
    public DataBaseItemViewModel(string title, DataBaseItemsType itemsType,
        object relatedObject, IEnumerable<(string title, Action action)> actions)
    {
        Title = title;
        ContextActions = actions.Select(x => new MenuItemViewModel(x.title, x.action)).ToArray();
        ItemsType = itemsType;
        RelatedObject = relatedObject;
    }

    public DataBaseItemViewModel(string title, DataBaseItemsType itemsType,
        object relatedObject, IEnumerable<(string title, Action action)> actions, ObservableCollection<DataBaseItemViewModel> subNodes)
    {
        Title = title;
        SubNodes = subNodes;
        RelatedObject = relatedObject;
        ItemsType = itemsType;
        ContextActions = actions.Select(x => new MenuItemViewModel(x.title, x.action)).ToArray();
    }

    public string Title { get; }
    public object RelatedObject { get; }
    public DataBaseItemsType ItemsType { get; }
    public ObservableCollection<DataBaseItemViewModel>? SubNodes { get; }
    public bool HasContextActions => ContextActions.Any();
    public IEnumerable<MenuItemViewModel> ContextActions { get; }
}