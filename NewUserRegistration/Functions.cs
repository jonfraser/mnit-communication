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
				
			await svc.ProcessServiceBusRegistrationMessage(
							CloudConfigurationManager.GetSetting("BaseWebUrl"),
							message);
		}
	}
}
