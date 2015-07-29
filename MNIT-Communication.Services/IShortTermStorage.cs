using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIT_Communication.Services
{
	public interface IShortTermStorage
	{
		Task StoreValue<T>(string key, T value, TimeSpan lifespan);
        
		Task<T> GetValue<T>(string key);

		Task<bool> KeyExists(string key);
	}
}
