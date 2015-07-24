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
        
        public async Task StoreKeyValue(string key, string value, TimeSpan lifespan)
        {
            cache.Add(key, value);
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

        public Task<bool> KeyExists(string key)
        {
            var exists = cache.ContainsKey(key) && !string.IsNullOrEmpty(cache[key]);
            return Task.FromResult(exists);
        }
    }
}
