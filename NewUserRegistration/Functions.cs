using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

namespace NewUserRegistration
{
	public class Functions
	{
		public async static Task ProcessQueueMessage([ServiceBusTrigger("RegistrationQueue")] NewUserRegistrationBrokeredMessage message, TextWriter log)
		{
			log.WriteLine("Received message from queue for " + message.EmailAddress);
			var svc = new RegistrationService();
			var endpoint = CloudConfigurationManager.GetSetting("BaseWebUrl");

			try
			{
				await svc.ProcessServiceBusRegistrationMessage(endpoint, message);
			}
			catch (Exception ex)
			{
				log.WriteLine(string.Format("Error in ProcessMessageQueue sending to {0}: {1}", endpoint, ex.ToString()));
				throw;
			}
		}
	}
}
