using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using MNIT_Communication.Domain;

namespace AlertWebsiteViaSignalRJob
{
	public class Functions
	{
		// This function will get triggered/executed when a new message is written 
		// on an Azure Queue called queue.
		public async static Task ProcessQueueMessage([ServiceBusTrigger(Topics.Alerts, Topics.Alerts + "-SignalR")] AlertBrokeredMessage message, TextWriter log)
		{
			log.WriteLine(message);

			//todo: lookup this alert
			//todo: send to hub
			
			//new MNIT_Communication.Hubs.OutageHub().SendNew(new AlertSummary
			//{
			//	Service = "TPCH Network",
			//	Update = "Still waiting for a miracle",
			//	Start = new DateTime(2015, 03, 10),
			//	UpdateDate = DateTime.Now
			//});
		}
	}
}
