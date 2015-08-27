using System;
using System.Linq;
using System.Threading.Tasks;
using MNIT_Communication.Domain;
using Newtonsoft.Json;

namespace MNIT_Communication.Services.Fakes
{
    public class MongoShortTermStorage : IShortTermStorage
    {
        private readonly IRepository repository;

        public MongoShortTermStorage(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task StoreValue<T>(string key, T value, TimeSpan lifespan)
        {
            if (await KeyExists(key))
                RemoveValue(key);

            var valueIsString = typeof(T) == typeof(string);

            var serializedValue = valueIsString ? value.ToString() : JsonConvert.SerializeObject(value);

            var toStore = new MongoCacheItem
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = serializedValue,
                StoredAt = DateTime.Now,
                LifeSpan = lifespan
            };

            await repository.Upsert(toStore);
        }

        public async Task<T> GetValue<T>(string key)
        {
            if (await KeyExists(key))
            {
                var item = (await repository.Get<MongoCacheItem>(i => i.Key == key)).First();

                var value = JsonConvert.DeserializeObject<T>(item.Value);
                return value;
            }

            return default(T);
        }

        private async Task RemoveValue(string key)
        {
            if (await KeyExists(key))
            {
                var toRemove = (await repository.Get<MongoCacheItem>(c => c.Key == key)).FirstOrDefault();
                await repository.Delete(toRemove); 
            }
        }

        public async Task<bool> KeyExists(string key)
        {
            var item = (await repository.Get<MongoCacheItem>(i => i.Key == key)).FirstOrDefault();
            return (item != null && !string.IsNullOrEmpty(item.Value));
        }
        
    }
    public class MongoCacheItem : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime StoredAt { get; set; }
        public TimeSpan LifeSpan { get; set; }
    }
}