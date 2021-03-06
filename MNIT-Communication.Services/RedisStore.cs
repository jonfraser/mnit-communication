﻿using System;
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

		public async Task StoreValue<T>(string key, T value, TimeSpan lifespan)
		{
            var serializedValue = JsonConvert.SerializeObject(value);
            await _cache.StringSetAsync(key, serializedValue, expiry: lifespan);
		}
        
		public async Task<T> GetValue<T>(string key)
		{
			var keyValue = await _cache.StringGetAsync(key);
			if(keyValue.IsNullOrEmpty)
			{
				return default(T);
			}

			return JsonConvert.DeserializeObject<T>(keyValue);
		}
        
		public async Task<bool> KeyExists(string key)
		{
			return await _cache.KeyExistsAsync(key);
		}
	}
}
