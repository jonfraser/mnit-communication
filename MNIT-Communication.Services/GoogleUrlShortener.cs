using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Urlshortener.v1.Data;
using Microsoft.WindowsAzure;

namespace MNIT_Communication.Services
{
	public class GoogleUrlShortener : IUrlShorten
	{
		public async Task<string> Shorten(string longUrl)
		{
			using (var shortener = new UrlshortenerService(new BaseClientService.Initializer
			{
				ApplicationName = "MNIT Communication",
				ApiKey = CloudConfigurationManager.GetSetting("GoogleApiKey")
			}))
			{
				var shortUrl = await shortener.Url.Insert(new Url { LongUrl = longUrl }).ExecuteAsync();
				return shortUrl.Id;
			}
		}
	}
}
