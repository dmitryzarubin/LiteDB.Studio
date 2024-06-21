using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using LiteDB.StudioNew.Models;

namespace LiteDB.StudioNew.Services;

public class ConnectionRepository : IConnectionRepository
{
    private readonly string _path;
    private readonly SourceCache<Connection, Guid> _sourceCache = new(c => c.Guid);

    public ConnectionRepository(string path)
    {
        _path = !string.IsNullOrEmpty(path) ? path : throw new ArgumentException("Connection file path is empty");

        ConnectionsObservable =  _sourceCache.Connect();
        
        GetAllAsync().ContinueWith(task =>
        {
            var connections = task.Result;
            _sourceCache.AddOrUpdate(connections);
        });
    }

    public IObservable<IChangeSet<Connection, Guid>> ConnectionsObservable { get; }

    public Connection? Get(Guid connectionGuid)
    {
        var connections = _sourceCache.KeyValues.Where(x => x.Key == connectionGuid).Select(x => x.Value).ToArray();
        if (connections.Length == 0)
            return null;

        return connections.Single();
    }

    public async Task InsertAsync(Connection connection)
    {
        if (_sourceCache.KeyValues.Select(x => x.Key).Any(x => x == connection.Guid))
            throw new InvalidOperationException("Connection with the same key is already exists");

        _sourceCache.AddOrUpdate(connection);

        await SaveToFile();
    }

    public async Task UpdateAsync(Connection connection)
    {
        if (_sourceCache.KeyValues.Select(x => x.Key).All(x => x != connection.Guid))
            throw new InvalidOperationException("Connection doesn't exist");

        _sourceCache.AddOrUpdate(connection);

        await SaveToFile();
    }

    public async Task RemoveAsync(Guid connectionGuid)
    {
        _sourceCache.Remove(connectionGuid);

        await SaveToFile();
    }

    private async Task<IReadOnlyList<Connection>> GetAllAsync()
    {
        await using var fileStream = File.Open(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        if (fileStream.Length == 0)
            return [];

        var connections = await System.Text.Json.JsonSerializer.DeserializeAsync<Connection[]>(fileStream);
        return connections!.AsReadOnly();
    }

    private async Task SaveToFile()
    {
        var alteredConnection = _sourceCache.KeyValues.Select(x => x.Value);
        await using var fileStream = File.Open(_path, FileMode.Create, FileAccess.Write, FileShare.None);
        await System.Text.Json.JsonSerializer.SerializeAsync(fileStream, alteredConnection);
    }
}