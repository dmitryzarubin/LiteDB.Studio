using System.Reactive;
using Avalonia.ReactiveUI;
using LiteDB.StudioNew.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace LiteDB.StudioNew.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        this.OneWayBind(ViewModel, vm => vm.Databases, v => v.TreeView.ItemsSource);
        this.OneWayBind(ViewModel, vm => vm.Queries, v => v.QueryTabControl.ItemsSource);
        
        this.BindCommand(ViewModel, vm => vm.ConnectCommand, v => v.ConnectButton);
        
        this.BindInteraction(ViewModel, vm => vm.Error, async context =>
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", context.Input, ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error).ShowAsync();

            context.SetOutput(Unit.Default);
        });
    }
}