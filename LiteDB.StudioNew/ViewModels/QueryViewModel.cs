using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LiteDB.StudioNew.Models;
using ReactiveUI;

namespace LiteDB.StudioNew.ViewModels;

public class QueryViewModel : ReactiveObject
{
    private readonly Collection _collection;
    private string _queryText;
    private string _textResult;

    public QueryViewModel(Collection collection, string? queryText = null)
    {
        _collection = collection;
        QueryText = queryText ?? string.Empty;

        ExecuteCommand = ReactiveCommand.CreateFromTask(Execute, outputScheduler: RxApp.TaskpoolScheduler);
    }

    public string Header { get; } = "Query 1";
    
    public string QueryText
    {
        get => _queryText;
        set => this.RaiseAndSetIfChanged(ref _queryText, value);
    }

    public string TextResult
    {
        get => _textResult;
        set => this.RaiseAndSetIfChanged(ref _textResult, value);
    }

    public ReactiveCommand<Unit, Unit> ExecuteCommand { get; }

    private async Task Execute()
    {
        var result = await _collection.Database.Execute(QueryText);

        var res = new StringBuilder();
        var stringWriter = new StringWriter(res);
        var writer = new JsonWriter(stringWriter)
        {
            Pretty = true,
            Indent = 2
        };

        var bsonArray = new BsonArray(result);
        writer.Serialize(bsonArray);

        RxApp.MainThreadScheduler.Schedule(1, (i, action) =>
        {
            TextResult = res.ToString();
        });

    }

    // Grid
}