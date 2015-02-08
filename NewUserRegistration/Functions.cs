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
		public async static Task ProcessQueueMessage([ServiceBusTrigger(Queues.Registration)] NewUserRegistrationBrokeredMessage message, TextWriter log)
		{
			log.WriteLine("Received message " + message.CorrelationId.ToString() + " from queue for " + message.EmailAddress);
			var svc = new RegistrationService();

			try
			{
				await svc.ProcessServiceBusRegistrationMessage(message);
			}
			catch (Exception ex)
			{
				log.WriteLine(string.Format("Error in ProcessMessageQueue: {0}", ex.ToString()));
				throw;
			}
		}
	}
}
