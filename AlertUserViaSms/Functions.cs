using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using MNIT_Communication.Domain;
using MNIT_Communication.Services;

namespace AlertUserViaSms
{
	public class Functions
	{
		public async static Task ProcessQueueMessage([ServiceBusTrigger(Topics.Alerts, Topics.Alerts+"-SMS")] AlertBrokeredMessage message, TextWriter log)
		{
			log.WriteLine(message);
		    var sms = ServiceLocator.Resolve<ISendSms>();
			var mobileNumber = "0416272575";
			//TODO:get teh mobile numbers to send to or would this actually just push a message onto anotehr
			//queue for each mobile number found? That would mean that this message process won't be dependant on
			//all messages succeeding
			await sms.SendSimple(mobileNumber, message.AlertInfoShort);
		}
	}
}
