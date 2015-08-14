using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using MNIT_Communication.Domain;
using MongoDB.Driver;

namespace MNIT_Communication.Services
{
    public class MongoDbRepository: IRepository
    {
        private IMongoClient client;
        private IMongoDatabase database;
        
        public MongoDbRepository()
        {
            var serverName = CloudConfigurationManager.GetSetting("MongoDBServer");
            var databaseName = CloudConfigurationManager.GetSetting("MongoDBDatabase");
            var connectionString = string.Format("{0}/{1}", serverName, databaseName);

            client = new MongoClient(connectionString);
            
            database = client.GetDatabase(databaseName);
        }
        
        public async Task<IList<T>> Get<T>() where T : BaseEntity
        {
            return await Get<T>(t => true); //Return all items
        }

        public async Task<T> Get<T>(Guid id) where T : BaseEntity
        {
            return (await Get<T>(t => t.Id == id)).FirstOrDefault();
        }

        public async Task<IList<T>> Get<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            if (predicate == null)
                return null;

            return await (await GetCursor(predicate)).ToListAsync();
        }

        private async Task<IAsyncCursor<T>> GetCursor<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            var collection = GetCollection<T>();

            var cursor = await collection.FindAsync(predicate);
            return cursor;
        }

        public IMongoCollection<T>  GetCollection<T>() where T : BaseEntity
        {
            var collectionName = typeof(T).Name;
            var collection = database.GetCollection<T>(collectionName);
            return collection;
        }

        public async Task<T> Upsert<T>(T item) where T : BaseEntity
        {
            var collection = GetCollection<T>();

            var args = new FindOneAndReplaceOptions<T>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var filter = Builders<T>.Filter.Where(x => x.Id == item.Id);
            
            return await collection.FindOneAndReplaceAsync(filter, item, args);

        }
       
    }
}
