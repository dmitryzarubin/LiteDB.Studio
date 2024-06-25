using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LiteDB.StudioNew.ViewModels;
using ReactiveUI;

namespace LiteDB.StudioNew.Views;

public partial class QueryView : ReactiveUserControl<QueryViewModel>
{
    public QueryView()
    {
        InitializeComponent();
        
        this.Bind(ViewModel, vm => vm.QueryText, v => v.TextEditor.Text);
        this.OneWayBind(ViewModel, vm => vm.TextResult, v => v.TextResult.Text);

        this.BindCommand(ViewModel, vm => vm.ExecuteCommand, v => v.ExecuteButton);
    }
}