using System;
using System.Threading.Tasks;
using DynamicData;
using LiteDB.StudioNew.Models;

namespace LiteDB.StudioNew.Services;

public interface IConnectionRepository
{
    IObservable<IChangeSet<Connection, Guid>> ConnectionsObservable { get; }

    Connection? Get(Guid connectionGuid);

    Task InsertAsync(Connection connection);
    Task UpdateAsync(Connection connection);
    Task RemoveAsync(Guid connectionGuid);
}