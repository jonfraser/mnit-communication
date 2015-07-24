using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

namespace VerifyMobileNumberJob
{
	public class Functions
	{
		public async static Task ProcessQueueMessage([ServiceBusTrigger(Queues.MobileNumberVerify)] VerifyMobileNumberBrokeredMessage message, TextWriter log)
		{
			log.WriteLine("Received message " + message.CorrelationId.ToString() + " from queue for " + message.MobileNumber);
		    var svc = ServiceLocator.Resolve<IRegistrationService>();

			try
			{
				await svc.VerifyMobileNumber(message.MobileNumber, message.NewUserRegistrationId);
			}
			catch (Exception ex)
			{
				log.WriteLine(string.Format("Error in ProcessMessageQueue: {0}", ex.ToString()));
				throw;
			}
		}
	}
}
