using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MNIT_Communication.Services
{
	public class RedisStore : IShortTermStorage
	{
		private readonly IDatabase _cache;

		public RedisStore()
		{
			ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(CloudConfigurationManager.GetSetting("RedisConnection"));
			_cache = connection.GetDatabase();
		}

		public async Task StoreKeyValue(string key, string value, TimeSpan lifespan)
		{
			await _cache.StringSetAsync(key, value, expiry: lifespan);
		}


		public async Task<T> GetValue<T>(string key)
		{
			return JsonConvert.DeserializeObject<T>(await _cache.StringGetAsync(key));
		}


		public async Task<bool> KeyExists(string key)
		{
			return await _cache.KeyExistsAsync(key);
		}
	}
}
