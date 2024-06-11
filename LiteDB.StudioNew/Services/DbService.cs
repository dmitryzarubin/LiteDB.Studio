using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiteDB.StudioNew
{
    public class DbService
    {
        private LiteDatabase _liteDatabase;

        public DbService()
        {
        }

        public async Task<LiteDatabase> ConnectAsync(ConnectionString connectionString)
        {
            return await Task.Run(() =>
            {
                _liteDatabase = new LiteDatabase(connectionString);
                
                // force open database
                var uv = _liteDatabase.UserVersion;
                
                return _liteDatabase;
            });
        }


        public IEnumerable<CollectionDto> GetCollections()
        {
            var systemCollections = _liteDatabase.GetCollection("$cols")
                .Query()
                .Where("type = 'system'")
                .OrderBy("name")
                .ToDocuments()
                .Select(doc => new CollectionDto
                {
                    Name = doc["name"].AsString,
                    IsSystem = true
                });


            var userCollections = _liteDatabase.GetCollectionNames().OrderBy(x => x)
                .Select(name => new CollectionDto
                {
                    Name = name,
                    IsSystem = false
                });

            return systemCollections.Union(userCollections).ToArray();
        }
    }
}