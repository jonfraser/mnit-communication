﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;

namespace MNIT_Communication.Services
{
	public class SendTwilioSmsService : ISendSms
	{
		public async Task SendSimple(string mobileNumber, string message)
		{

			var twilioSmsPrefix = "Sent from your twilio trial account - ";
			if (twilioSmsPrefix.Length + message.Length > 160)
			{
				throw new ArgumentException("SMS Message must be less than 120 but it was " + message.Length.ToString());
			}
			var sms = new Twilio.TwilioRestClient("ACcff9c328336b5cd4c892e9d87905d3d4", CloudConfigurationManager.GetSetting("TwilioPassword"));

			sms.SendSmsMessage("+19073122358", mobileNumber, message);
		}
	}
}