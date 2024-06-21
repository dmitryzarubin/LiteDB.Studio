using System.Reactive;
using Avalonia.ReactiveUI;
using LiteDB.StudioNew.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace LiteDB.StudioNew.Views;

public partial class ConnectionListView : ReactiveWindow<ConnectionListViewModel>
{
    public ConnectionListView()
    {
        InitializeComponent();

        this.WhenActivated(disposeAction =>
        {
            disposeAction(this.OneWayBind(ViewModel, vm => vm.Connections, v => v.ConnectionsDataGrid.ItemsSource));
        });

        this.Bind(ViewModel, vm => vm.SelectedConnection, v => v.ConnectionsDataGrid.SelectedItem);

        this.BindCommand(ViewModel, vm => vm.AddConnectionCommand, view => view.AddButton);
        this.BindCommand(ViewModel, vm => vm.EditConnectionCommand, view => view.EditButton);
        this.BindCommand(ViewModel, vm => vm.RemoveConnectionCommand, view => view.RemoveButton);

        this.BindInteraction(ViewModel, vm => vm.Error, async context =>
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", context.Input, ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error).ShowAsync();

            context.SetOutput(Unit.Default);
        });

        this.BindInteraction(ViewModel, vm => vm.ConfirmDeletion, async context =>
        {
            var result = await MessageBoxManager.GetMessageBoxStandard("Confirm", "Remove the selected connection?",
                ButtonEnum.YesNo,
                MsBox.Avalonia.Enums.Icon.Error).ShowAsync();

            context.SetOutput(result == ButtonResult.Yes);
        });
    }
}