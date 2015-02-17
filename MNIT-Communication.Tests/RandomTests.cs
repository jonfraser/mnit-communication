using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MNIT_Communication.Services;

namespace MNIT_Communication.Tests
{
	[TestClass]
	public class RandomTests
	{
		[TestMethod]
		public async Task CanSendSmsThroughTelstra()
		{
			ISendSms sms = new SendTelstraSmsService();
			await sms.SendSimple("0466646967", "Love you! This is what I'm playing with :p");
		}
	}
}
