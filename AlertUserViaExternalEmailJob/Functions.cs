using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

namespace AlertUserViaExternalEmailJob
{
	public class Functions
	{
		public async static Task ProcessQueueMessage([ServiceBusTrigger(Topics.Alerts, Topics.Alerts + "-SMS")] AlertBrokeredMessage message, TextWriter log)
		{
			log.WriteLine(message);
			ISendEmail mail = new SendGridEmailService();
			await mail.Send(from: "mnit-communication@health.qld.gov.au",
										to: new List<String> { "fraser.jc@gmail.com" },
										subject: "An alert has been raised!",
										body: message.AlertDetail);
		}
	}
}
