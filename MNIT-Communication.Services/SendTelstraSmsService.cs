using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;

namespace MNIT_Communication.Services
{
	public class SendTelstraSmsService : ISendSms
	{
		public async Task SendSimple(string mobileNumber, string message)
		{
			using(var client = new HttpClient())
			{
				var oAuthJson = await client.GetStringAsync(string.Format("https://api.telstra.com/v1/oauth/token?client_id={0}&client_secret={1}&grant_type=client_credentials&scope=SMS",
					CloudConfigurationManager.GetSetting("TelstraSmsApiKey"),
					CloudConfigurationManager.GetSetting("TelstraSmsApiSecret")));

				var tokenDefinition = new { access_token = "", expires_in = "" };
				var oAuth = JsonConvert.DeserializeAnonymousType(oAuthJson, tokenDefinition);

				var sms = new 
					{
						to = mobileNumber,
						body = message
					};
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oAuth.access_token);
				await client.PostAsync("https://api.telstra.com/v1/sms/messages", new StringContent(JsonConvert.SerializeObject(sms)));
			}

			
		}
	}
}
