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

namespace MNIT_Communication.Jobs.NewUserRegistration
{
	public class Functions
	{

		// This function will get triggered/executed when a new message is written 
		// on an Azure Queue called queue.
		public static void ProcessQueueMessage([QueueTrigger("RegistrationQueue")] NewUserRegistrationBrokeredMessage message, TextWriter log)
		{
			log.WriteLine("Received message from queue");
			new RegistrationService().ProcessServiceBusRegistrationMessage(
							CloudConfigurationManager.GetSetting("BaseWebUrl"),
							message).RunSynchronously();
		}
	}
}
