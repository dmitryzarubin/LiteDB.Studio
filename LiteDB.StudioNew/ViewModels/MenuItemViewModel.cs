using System;
using System.Reactive;
using ReactiveUI;

namespace LiteDB.StudioNew.ViewModels;

public class MenuItemViewModel(string title, Action action) : ReactiveObject
{
    public string Title { get; } = title;

    public ReactiveCommand<Unit, Unit> ActionCommand { get; } = ReactiveCommand.Create(action);
}