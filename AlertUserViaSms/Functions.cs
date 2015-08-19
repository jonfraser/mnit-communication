using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using MNIT.ErrorLogging;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

namespace AlertUserViaSms
{
	public class Functions
	{
		public async static Task ProcessQueueMessage([ServiceBusTrigger(Topics.Alerts, Topics.Alerts+"-SMS")] AlertBrokeredMessage message, TextWriter log)
		{
			log.WriteLine(message);

            var errorLogger = ServiceLocator.Resolve<IErrorLogger<Guid>>();
           
            var alertsService = ServiceLocator.Resolve<IAlertsService>();
            var subscribers = await alertsService.GetSubscribersFor(message.AlertableId);
            var sms = ServiceLocator.Resolve<ISendSms>();
            
			//queue for each mobile number found? That would mean that this message process won't be dependant on
			//all messages succeeding
			
            foreach (var subscriber in subscribers.Where(s => !string.IsNullOrEmpty(s.MobilePhoneNumber))) //Only users who have supplied a Mobile Number
            {
                try
                {
                    await sms.SendSimple(subscriber.MobilePhoneNumber, message.AlertInfoShort);
                }
                catch(Exception ex)
                {
                    errorLogger.LogError(ex);
                }
            }
		}
	}
}
