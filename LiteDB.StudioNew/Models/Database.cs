using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB.StudioNew.Models;
using Index = LiteDB.StudioNew.Models.Index;

namespace LiteDB.StudioNew.Services;

public class Database
{
    private readonly Connection _connection;
    private readonly ConnectionString _connectionString;
    private LiteDatabase _liteDatabase;

    public Database(Connection connection)
    {
        _connection = connection;

        _connectionString = new ConnectionString
        {
            Connection = connection.ConnectionType == Models.ConnectionType.Direct
                ? ConnectionType.Direct
                : ConnectionType.Shared,
            Filename = connection.Path,
            Password = connection.Password,
            ReadOnly = connection.ReadOnly,
            Upgrade = connection.Upgrade,
            InitialSize = connection.InitialSize,
            Collation = CreateCollation(connection)
        };
    }

    public string ConnectionName => _connection.Name;

    public IEnumerable<Collection> Collections { get; private set; }

    private static Collation? CreateCollation(Connection connection)
    {
        if (string.IsNullOrEmpty(connection.Culture))
            return null;

        return string.IsNullOrEmpty(connection.Sort)
            ? new Collation(connection.Culture)
            : new Collation(connection.Culture + "/" + connection.Sort);
    }

    public void Connect()
    {
        _liteDatabase = new LiteDatabase(_connectionString);

        Collections = GetCollections();
    }
    
    public void Disconnect()
    {
        _liteDatabase.Dispose();
        Collections = Array.Empty<Collection>();
    }


    
    public async Task<List<BsonDocument>> Execute(string query)
    {
        var values = await Task.Run( () =>
        {
            var bsonDataReader = _liteDatabase.Execute(query).ToArray();
            var bsonValues = bsonDataReader.ToArray();
            return bsonValues;
        });

        return values.Cast<BsonDocument>().ToList();
    }

    private IEnumerable<Collection> GetCollections()
    {
        var systemCollections = _liteDatabase.GetCollection("$cols")
            .Query()
            .Where("type = 'system'")
            .ToDocuments()
            .Select(doc => new Collection
            {
                Name = doc["name"].AsString,
                IsSystem = true,
                Indices = Array.Empty<Index>(),
                Database = this
            });

        var userCollections = _liteDatabase.GetCollectionNames()
            .Select(name => new Collection
            {
                Name = name,
                IsSystem = false,
                Indices = _liteDatabase.GetCollection<Index>("$indexes").Query().Where("collection = " + name).ToArray(),
                Database = this
            });

        return systemCollections.Union(userCollections).ToArray();
    }
}