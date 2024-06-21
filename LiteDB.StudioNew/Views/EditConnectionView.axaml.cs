using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using LiteDB.StudioNew.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace LiteDB.StudioNew.Views;

public partial class EditConnectionView : ReactiveWindow<EditConnectionViewModel>
{
    public EditConnectionView()
    {
        InitializeComponent();

        this.Bind(ViewModel, vm => vm.Name, v => v.ConnectionNameTextBox.Text);
        this.Bind(ViewModel, vm => vm.Path, v => v.PathTextBox.Text);
        this.Bind(ViewModel, vm => vm.Password, v => v.PasswordTextBox.Text);


        this.Bind(ViewModel, vm => vm.ConnectionType, v => v.DirectRadioButton.IsChecked,
            t => t == Models.ConnectionType.Direct, b => b == true ? Models.ConnectionType.Direct : Models.ConnectionType.Shared);
        this.Bind(ViewModel, vm => vm.ConnectionType, v => v.SharedRadioButton.IsChecked,
            t => t == Models.ConnectionType.Shared, b => b == true ? Models.ConnectionType.Shared : Models.ConnectionType.Direct);

        this.Bind(ViewModel, vm => vm.ReadOnly, v => v.ReadOnlyCheckBox.IsChecked);
        this.Bind(ViewModel, vm => vm.Upgrade, v => v.UpgradeCheckBox.IsChecked);
        this.Bind(ViewModel, vm => vm.InitialSize, v => v.InitialSizeNumericUpDown.Value, i => i, d => (int)(d ?? 0));
        this.OneWayBind(ViewModel, vm => vm.CulturesList, v => v.CultureComboBox.ItemsSource);
        this.OneWayBind(ViewModel, vm => vm.SortsList, v => v.SortComboBox.ItemsSource);
        this.Bind(ViewModel, vm => vm.Culture, v => v.CultureComboBox.SelectedItem);
        this.Bind(ViewModel, vm => vm.Sort, v => v.SortComboBox.SelectedItem);

        this.BindCommand(ViewModel, vm => vm.UpdateConnectionCommand, v => v.EditButton);
        this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.CloseButton);

        this.BindInteraction(ViewModel, vm => vm.Error, async context =>
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", context.Input, ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error).ShowAsync();

            context.SetOutput(Unit.Default);
        });
    }


    private void SelectFile_OnClick(object? sender, RoutedEventArgs e)
    {
        Task.Run(async () =>
        {
            var topLevel = GetTopLevel(this);

            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open database",
                AllowMultiple = false
            });

            if (files.Any() && files.SingleOrDefault() != null)
                RxApp.MainThreadScheduler.Schedule(Unit.Default, (_, _) =>
                {
                    var storageFile = files.Single();
                    PathTextBox.Text = storageFile.TryGetLocalPath();
                    return Disposable.Empty;
                });
        });
    }
}