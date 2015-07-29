using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MNIT_Communication.Services.Fakes
{
    public class FakeShortTermStorage : IShortTermStorage
    {
        private static Dictionary<string, string> cache = new Dictionary<string, string>();
        
        public async Task StoreValue<T>(string key, T value, TimeSpan lifespan)
        {
            if (await KeyExists(key))
                RemoveValue(key);

            var valueIsString = typeof (T) == typeof (string);

            var serializedValue = valueIsString ? value.ToString(): JsonConvert.SerializeObject(value);

            cache.Add(key, serializedValue);
        }

        public async Task<T> GetValue<T>(string key)
        {
            if (await KeyExists(key))
            {
                var value = JsonConvert.DeserializeObject<T>(cache[key]);
                return value;
            }

            return default(T);
        }

        private void RemoveValue(string key)
        {
            cache.Remove(key);
        }

        public Task<bool> KeyExists(string key)
        {
            var exists = cache.ContainsKey(key) && !string.IsNullOrEmpty(cache[key]);
            return Task.FromResult(exists);
        }

        
    }
}
