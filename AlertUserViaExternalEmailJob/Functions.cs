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

namespace AlertUserViaExternalEmailJob
{
	public class Functions
	{

        public async static Task ProcessQueueMessage([ServiceBusTrigger(Topics.Alerts, Topics.Alerts + "-ExternalEmail")] AlertBrokeredMessage message, TextWriter log)
		{
			log.WriteLine(message);

            var errorLogger = ServiceLocator.Resolve<IErrorLogger>();

            var mail = ServiceLocator.Resolve<ISendEmail>();
		    var alertsService = ServiceLocator.Resolve<IAlertsService>();
		    var subscribers = await alertsService.GetSubscribersFor(message.AlertableId);

            
            foreach (var subscriber in subscribers)
		    {
		        try
		        {
		            var subscriberAddresses = new List<string> {subscriber.EmailAddressExternalProvider, subscriber.EmailAdressInternal};
                    
                    await mail.Send(from: "mnit-communication@health.qld.gov.au",
									to: subscriberAddresses,
									subject: "An alert has been raised!",
									body: message.AlertDetail);

		        }
		        catch(Exception ex)
		        {
		            errorLogger.LogError(ex);
		        }
		    }
            
            
		}
	}
}
