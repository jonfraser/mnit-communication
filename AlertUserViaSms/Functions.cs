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
            var auditService = ServiceLocator.Resolve<IAuditService>();

            var alertsService = ServiceLocator.Resolve<IAlertsService>();
            var subscribers = await alertsService.GetSubscribersFor(message.AlertableId);
            var sms = ServiceLocator.Resolve<ISendSms>();
            
			//queue for each mobile number found? That would mean that this message process won't be dependant on
			//all messages succeeding
			
            foreach (var subscriber in subscribers.Where(s => !string.IsNullOrEmpty(s.MobilePhoneNumber))) //Only users who have supplied a Mobile Number
            {
                try
                {
                    var body = string.Format(@"MNHHS Communication Alert has been {0}: '{1} by {2}'. \0x0A {3}", // '\0x0A' is a line break
                        message.AlertStatus, 
                        message.AlertInfoShort, 
                        message.AlertRaiser.Name, 
                        message.AlertDetail);

                    await sms.SendSimple(subscriber.MobilePhoneNumber, body);
                }
                catch(Exception ex)
                {
                    errorLogger.LogError(ex);
                }
            }

            auditService.LogAuditEvent(new AuditEvent
            {
                AuditType = AuditType.WebJobMessageProcessing,
                Details = "A message has been recieved by the AlertUserViaExternalEmailJob for processing and email has been sent to: " + String.Join(", ", subscribers.Select(s => s.EmailAdressInternal + "," + s.EmailAddressExternalProvider).ToArray()),
                Data = message
            });
        }
	}
}
